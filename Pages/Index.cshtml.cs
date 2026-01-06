// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using BetsiApp.Models;
using BetsiApp.Services;
using BetsiApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BetsiApp.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly FootballApiService _apiService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(FootballApiService apiService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _apiService = apiService;
            _context = context;
            _userManager = userManager;
        }

        public List<Match> Matches { get; set; } = new List<Match>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            Matches = await _apiService.GetUpcomingMatchesAsync();
        }

        // Handler, ki obdela oddajo stave
        public async Task<IActionResult> OnPostPlaceBetAsync(int matchId, string homeTeam, string awayTeam, string selectedOutcome, decimal oddValue, decimal stake)
        {
            if (stake <= 0)
            {
                StatusMessage = "Error: Znesek stave mora biti večji od 0.";
                return RedirectToPage();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (user.Balance < stake)
            {
                StatusMessage = "Error: Nimate dovolj sredstev (Stanje: " + user.Balance + " €).";
                return RedirectToPage();
            }

            // 1. Odštejemo denar uporabniku
            user.Balance -= stake;

            // 2. Ustvarimo stavni listič
            var betSlip = new BetSlip
            {
                UserId = user.Id,
                Stake = stake,
                TotalOdd = oddValue,
                PlacementTime = DateTime.UtcNow,
                Status = "OPEN"
            };

            // 3. Ustvarimo postavko na lističu
            var betItem = new BetItem
            {
                MatchId = matchId,
                MatchDescription = $"{homeTeam} vs {awayTeam}",
                SelectedOutcome = selectedOutcome,
                PlacedOdd = oddValue
            };

            betSlip.BetItems.Add(betItem);

            _context.BetSlips.Add(betSlip);
            await _context.SaveChangesAsync();

            StatusMessage = $"Stava uspešno oddana! Stavili ste {stake} € na tip {selectedOutcome} ({homeTeam} vs {awayTeam}).";
            return RedirectToPage();
        }
    }
}