// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using BetsiApp.Models;
using BetsiApp.Services;
using Microsoft.AspNetCore.Authorization; 

namespace BetsiApp.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly FootballApiService _apiService;

        // DI: Storitev se injektira v konstruktor
        public IndexModel(FootballApiService apiService)
        {
            _apiService = apiService;
        }

        // Lastnost za shranjevanje pridobljenih tekem
        public List<Match> Matches { get; set; } = new List<Match>();

        public async Task OnGetAsync()
        {
            // Kličemo servis in pridobimo podatke, ko se stran naloži
            Matches = await _apiService.GetUpcomingMatchesAsync();
        }
    }
}