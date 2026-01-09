using BetsiApp.Data;
using BetsiApp.Models;
using BetsiApp.Pages;
using Microsoft.EntityFrameworkCore;

namespace BetsiApp.Services
{
    public class BetSettlingService
    {
        private readonly ApplicationDbContext _context;
        private readonly FootballApiService _apiService;
        private readonly NotificationService _notificationService;
        private readonly BasketballApiService _basketballApiService;

        public BetSettlingService(ApplicationDbContext context, FootballApiService apiService, NotificationService notificationService, BasketballApiService basketballApiService)
        {
            _context = context;
            _apiService = apiService;
            _notificationService = notificationService;
            _basketballApiService = basketballApiService;
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
            var finishedBasketballGames = await _basketballApiService.GetFinishedGamesAsync();


            // 3. Pridobi uporabnike v slovar
            var usersDict = await _context.Users.ToDictionaryAsync(u => u.Id);

            foreach (var slip in openSlips)
            {
                bool allItemsSettled = true;
                bool slipWon = true;

                foreach (var item in slip.BetItems)
                {
                    bool? itemWon = null;

                    // =====================
                    // ⚽ FOOTBALL
                    // =====================
                    if (item.Sport == "Football")
                    {
                        var match = finishedMatches
                            .FirstOrDefault(m => m.Id == item.MatchId && m.Status == "FINISHED");

                        if (match?.Score?.FullTime?.Home != null &&
                            match.Score.FullTime.Away != null)
                        {
                            string actualOutcome =
                                DetermineOutcome(
                                    match.Score.FullTime.Home.Value,
                                    match.Score.FullTime.Away.Value);

                            itemWon = item.SelectedOutcome == actualOutcome;
                        }
                    }

                    // =====================
                    // 🏀 BASKETBALL
                    // =====================
                    else if (item.Sport == "Basketball")
                    {
                        var game = finishedBasketballGames
                            .FirstOrDefault(g => g.Id == item.MatchId);

                        if (game?.Scores?.Home?.Total != null &&
                            game.Scores.Away.Total != null)
                        {
                            string winner =
                                game.Scores.Home.Total > game.Scores.Away.Total
                                    ? game.Teams.Home.Name
                                    : game.Teams.Away.Name;

                            itemWon = item.SelectedOutcome == winner;
                        }
                    }

                    // =====================
                    // Skupna obdelava
                    // =====================
                    if (itemWon == null)
                    {
                        allItemsSettled = false;
                    }
                    else if (itemWon == false)
                    {
                        slipWon = false;
                    }
                }

                // =====================
                // Zaključi listič
                // =====================
                if (!string.IsNullOrEmpty(slip.UserId) &&
                    usersDict.TryGetValue(slip.UserId, out var user))
                {
                    if (allItemsSettled)
                    {
                        if (slipWon)
                        {
                            slip.Status = "WON";

                            var winnings = slip.Stake * slip.TotalOdd;
                            user.Balance += winnings;

                            _context.Transactions.Add(new Transaction
                            {
                                Amount = winnings,
                                Date = DateTime.Now,
                                Type = "Winnings",
                                Description = $"Dobitek stave #{slip.Id}",
                                UserId = user.Id
                            });

                            await _notificationService.CreateNotificationAsync(
                                user.Id,
                                $"🎉 Čestitamo! Vaša stava #{slip.Id} je zmagala. Dobitek: {winnings:F2} €");
                        }
                        else
                        {
                            slip.Status = "LOST";

                            await _notificationService.CreateNotificationAsync(
                                user.Id,
                                $"💔 Žal vaša stava #{slip.Id} ni bila uspešna.");
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