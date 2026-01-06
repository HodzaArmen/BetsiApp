using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BetsiApp.Data;
using BetsiApp.Models;
using BetsiApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BetsiApp.Pages
{
    [Authorize]
    public class MyBetsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyBetsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<BetSlip> BetSlips { get; set; } = new List<BetSlip>();

        public async Task OnGetAsync()
        {
            // Najprej sprožimo preverjanje rezultatov in dodeljevanje dobitkov
            var settlingService = HttpContext.RequestServices.GetRequiredService<BetSettlingService>();
            await settlingService.SettleBetsAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                BetSlips = await _context.BetSlips
                    .Include(b => b.BetItems)
                    .Where(b => b.UserId == user.Id)
                    .OrderByDescending(b => b.PlacementTime)
                    .ToListAsync();
            }
        }
    }
}