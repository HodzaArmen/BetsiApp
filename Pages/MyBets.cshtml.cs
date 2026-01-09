using Microsoft.AspNetCore.Mvc;
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
        private readonly NotificationService _notificationService;

        public MyBetsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, NotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public List<BetSlip> BetSlips { get; set; } = new List<BetSlip>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public async Task OnGetAsync()
        {
            // Preverjanje rezultatov in dodeljevanje dobitkov
            var settlingService = HttpContext.RequestServices.GetRequiredService<BetSettlingService>();
            await settlingService.SettleBetsAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Lističi
                BetSlips = await _context.BetSlips
                    .Include(b => b.BetItems)
                    .Where(b => b.UserId == user.Id)
                    .OrderByDescending(b => b.PlacementTime)
                    .ToListAsync();

                // Obvestila
                Notifications = await _notificationService.GetNotificationsAsync(user.Id);
            }
        }

        public async Task<IActionResult> OnPostMarkReadAsync(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMarkAllReadAsync()
        {
            var userId = _userManager.GetUserId(User);
            await _notificationService.MarkAllAsReadAsync(userId);
            return RedirectToPage();
        }
    }
}