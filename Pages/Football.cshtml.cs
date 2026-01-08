using Microsoft.AspNetCore.Mvc.RazorPages;
using BetsiApp.Services;
using BetsiApp.Models;
using BetsiApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BetsiApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace BetsiApp.Pages
{
    public class FootballModel : PageModel
    {
        private readonly FootballApiService _footballService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;



        public FootballModel(
            FootballApiService footballService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _footballService = footballService;
            _context = context;
            _userManager = userManager;
        }



        public List<Match> UpcomingMatches { get; set; } = new();

        public async Task OnGet()
        {
            UpcomingMatches = await _footballService.GetUpcomingMatchesAsync();
        }

        [Authorize]
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
                TempData["StatusMessage"] = "Error: Znesek stave mora biti večji od 0.";
                return RedirectToPage();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (user.Balance < stake)
            {
                TempData["StatusMessage"] =
                    $"Error: Nimate dovolj sredstev (Stanje: {user.Balance} €).";
                return RedirectToPage();
            }

            // 1. Odštejemo denar
            user.Balance -= stake;

            // 2. Stavni listič
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

            // 3. Postavka
            var betItem = new BetItem
            {
                MatchId = matchId,
                MatchDescription = $"{homeTeam} vs {awayTeam}",
                SelectedOutcome = outcomeText,
                PlacedOdd = oddValue,
                Sport = "Football"
            };

            betSlip.BetItems.Add(betItem);

            _context.BetSlips.Add(betSlip);
            await _context.SaveChangesAsync();


            TempData["StatusMessage"] =
                $"Stava uspešno oddana ⚽ | {stake} € na {outcomeText}";

            return RedirectToPage();
        }

    }
    
}
