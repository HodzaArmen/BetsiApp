using Microsoft.AspNetCore.Mvc.RazorPages;
using BetsiApp.Models;
using BetsiApp.Services;

namespace BetsiApp.Pages
{
    public class BasketballModel : PageModel
    {
        private readonly BasketballApiService _basketballApiService;

        public BasketballModel(BasketballApiService basketballApiService)
        {
            _basketballApiService = basketballApiService;
        }

        public List<BasketballGame> UpcomingGames { get; set; } = new();

        public async Task OnGet()
        {
            UpcomingGames = await _basketballApiService.GetUpcomingGamesAsync();
        }
    }
}
