using BetsiApp.Data;
using BetsiApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BetsiApp.Services
{
    public class BetSettlingService
    {
        private readonly ApplicationDbContext _context;
        private readonly FootballApiService _apiService;
        private readonly NotificationService _notificationService;

        public BetSettlingService(ApplicationDbContext context, FootballApiService apiService, NotificationService notificationService)
        {
            _context = context;
            _apiService = apiService;
            _notificationService = notificationService;
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

            // 3. Pridobi uporabnike v slovar
            var usersDict = await _context.Users.ToDictionaryAsync(u => u.Id);

            foreach (var slip in openSlips)
            {
                bool allItemsSettled = true;
                bool slipWon = true;

                foreach (var item in slip.BetItems)
                {
                    var matchResult = finishedMatches.FirstOrDefault(m => m.Id == item.MatchId && m.Status == "FINISHED");

                    if (matchResult?.Score?.FullTime?.Home != null && matchResult.Score.FullTime.Away != null)
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

                if (!string.IsNullOrEmpty(slip.UserId) && usersDict.TryGetValue(slip.UserId, out var user))
                {
                    if (allItemsSettled)
                    {
                        if (slipWon)
                        {
                            slip.Status = "WON";
                            var winnings = slip.Stake * slip.TotalOdd;
                            user.Balance += winnings;

                            // Log transaction for winnings
                            var transaction = new Transaction
                            {
                                Amount = winnings,
                                Date = DateTime.Now,
                                Type = "Winnings",
                                Description = $"Dobitek stave #{slip.Id}",
                                UserId = user.Id
                            };
                            _context.Transactions.Add(transaction);

                            await _notificationService.CreateNotificationAsync(user.Id, $"🎉 Čestitamo! Vaša stava #{slip.Id} je zmagala. Dobitek: {winnings:F2} €");
                        }
                        else
                        {
                            slip.Status = "LOST";
                            await _notificationService.CreateNotificationAsync(user.Id, $"💔 Žal vaša stava #{slip.Id} ni bila uspešna.");
                        }
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