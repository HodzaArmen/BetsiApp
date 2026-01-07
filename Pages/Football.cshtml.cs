using Microsoft.AspNetCore.Mvc.RazorPages;
using BetsiApp.Services;
using BetsiApp.Models;

namespace BetsiApp.Pages
{
    public class FootballModel : PageModel
    {
        private readonly FootballApiService _footballService;

        public FootballModel(FootballApiService footballService)
        {
            _footballService = footballService;
        }

        public List<Match> UpcomingMatches { get; set; } = new();

        public async Task OnGet()
        {
            UpcomingMatches = await _footballService.GetUpcomingMatchesAsync();
        }
    }
}
