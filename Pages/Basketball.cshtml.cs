using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BetsiApp.Models;
using BetsiApp.Services;
using BetsiApp.Data;

namespace BetsiApp.Pages
{
    [Authorize]
    public class BasketballModel : PageModel
    {
        private readonly BasketballApiService _basketballApiService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BasketballModel(
            BasketballApiService basketballApiService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _basketballApiService = basketballApiService;
            _context = context;
            _userManager = userManager;
        }

        public List<BasketballGame> UpcomingGames { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? League { get; set; }

        public List<string> LeagueOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            var games = await _basketballApiService.GetUpcomingGamesAsync();

            LeagueOptions = games
                .Select(g => g.League?.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .OrderBy(n => n)
                .ToList()!;

            if (!string.IsNullOrWhiteSpace(League))
            {
                games = games
                    .Where(g => g.League?.Name == League)
                    .ToList();
            }

            UpcomingGames = games;
        }

        // 🏀 STAVNI HANDLER
        public async Task<IActionResult> OnPostPlaceBetAsync(
            int matchId,
            string homeTeam,
            string awayTeam,
            string selectedOutcome,
            decimal oddValue,
            decimal stake)
        {
            if (stake <= 0)
            {
                TempData["StatusMessage"] = "Napaka: Znesek mora biti večji od 0.";
                return RedirectToPage();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login");

            if (user.Balance < stake)
            {
                TempData["StatusMessage"] =
                    $"Napaka: Nimate dovolj sredstev (Stanje: {user.Balance} €).";
                return RedirectToPage();
            }

            // 1️⃣ Odštejemo znesek
            user.Balance -= stake;

            // 2️⃣ Stavni listič
            var betSlip = new BetSlip
            {
                UserId = user.Id,
                Stake = stake,
                TotalOdd = oddValue,
                PlacementTime = DateTime.UtcNow,
                Status = "OPEN"
            };

            string outcomeText;

            if (selectedOutcome == "X")
            {
                outcomeText = "Neodločeno";
            }
            else if (selectedOutcome == "1")
            {
                outcomeText = homeTeam;
            }
            else if (selectedOutcome == "2")
            {
                outcomeText = awayTeam;
            }
            else
            {
                outcomeText = selectedOutcome;
            }

            // 🔹 Zabeleži stavo v zgodovino transakcij
            var transaction = new Transaction
            {
                UserId = user.Id,
                Amount = stake,
                Date = DateTime.UtcNow,
                Type = "Bet",
                Description = $"Stava: {homeTeam} vs {awayTeam} – {outcomeText}"
            };

            _context.Transactions.Add(transaction);


            // 3️⃣ Postavka
            var betItem = new BetItem
            {
                MatchId = matchId,
                MatchDescription = $"{homeTeam} vs {awayTeam}",
                SelectedOutcome = outcomeText,
                PlacedOdd = oddValue,
                Sport = "Basketball"
            };

            betSlip.BetItems.Add(betItem);

            _context.BetSlips.Add(betSlip);
            await _context.SaveChangesAsync();


            TempData["StatusMessage"] =
                $"Stava uspešno oddana 🏀 | {stake} € na {outcomeText}";

            return RedirectToPage();
        }
    }
}
