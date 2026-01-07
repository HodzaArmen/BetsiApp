using BetsiApp.Data;
using BetsiApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BetsiApp.Services
{
    public class BetSettlingService
    {
        private readonly ApplicationDbContext _context;
        private readonly FootballApiService _apiService;

        public BetSettlingService(ApplicationDbContext context, FootballApiService apiService)
        {
            _context = context;
            _apiService = apiService;
        }

        public async Task SettleBetsAsync()
        {
            // 1. Pridobi vse odprte lističe
            var openSlips = await _context.BetSlips
                .Include(b => b.BetItems)
                .Where(b => b.Status == "OPEN")
                .ToListAsync();

            if (!openSlips.Any()) return;

            // 2. Pridobi rezultate tekem za zadnjih 3 dni iz API-ja
            var finishedMatches = await _apiService.GetMatchesByDateRangeAsync(DateTime.Today.AddDays(-3), DateTime.Today);

            foreach (var slip in openSlips)
            {
                bool allItemsSettled = true;
                bool slipWon = true;

                foreach (var item in slip.BetItems)
                {
                    var matchResult = finishedMatches.FirstOrDefault(m => m.Id == item.MatchId && m.Status == "FINISHED");

                    if (matchResult != null && matchResult.Score?.FullTime?.Home != null)
                    {
                        string actualOutcome = DetermineOutcome(matchResult.Score.FullTime.Home.Value, matchResult.Score.FullTime.Away.Value);
                        
                        // Preveri, če je uporabnik uganil
                        if (item.SelectedOutcome != actualOutcome)
                        {
                            slipWon = false;
                        }
                    }
                    else
                    {
                        // Tekma še ni končana
                        allItemsSettled = false;
                    }
                }

                if (allItemsSettled)
                {
                    if (slipWon)
                    {
                        slip.Status = "WON";
                        // Avtomatsko dodajanje denarja uporabniku
                        var user = await _context.Users.FindAsync(slip.UserId);
                        if (user != null)
                        {
                            user.Balance += (slip.Stake * slip.TotalOdd);
                        }
                    }
                    else
                    {
                        slip.Status = "LOST";
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private string DetermineOutcome(int homeGoals, int awayGoals)
        {
            if (homeGoals > awayGoals) return "1";
            if (awayGoals > homeGoals) return "2";
            return "X";
        }
    }
}