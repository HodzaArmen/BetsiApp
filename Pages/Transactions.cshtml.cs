using BetsiApp.Data;
using BetsiApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BetsiApp.Pages
{
    public class TransactionsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public TransactionsModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public decimal Balance { get; set; }

        [TempData]
        public string? ToastMessage { get; set; }

        public string? InlineError { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }

        [BindProperty]
        public bool ShowAll { get; set; }

        public List<Transaction> History { get; set; } = new();


        public async Task<IActionResult> OnPostShowAllAsync()
        {
            ShowAll = true;

            var user = await _userManager.GetUserAsync(User);
            Balance = user?.Balance ?? 0.00m;

            if (user != null)
            {
                History = await _db.Transactions
                    .Where(t => t.UserId == user.Id)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
            }

            return Page();
        }
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Balance = user?.Balance ?? 0.00m;

            if (user != null)
            {
                History = await _db.Transactions
                    .Where(t => t.UserId == user.Id)
                    .OrderByDescending(t => t.Date)
                    .Take(5)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAddMoneyAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                InlineError = "Uporabnik ni prijavljen.";
                await OnGetAsync();
                return Page();
            }

            if (Amount <= 0)
            {
                InlineError = "Prosim, vnesite veljaven znesek.";
                await OnGetAsync();
                return Page();
            }

            user.Balance += Amount;
            await _userManager.UpdateAsync(user);

            _db.Transactions.Add(new Transaction
            {
                UserId = user.Id,
                Amount = Amount,
                Date = DateTime.UtcNow,
                Type = "Deposit",
                Description = $"Polog {Amount:F2} €"
            });

            await _db.SaveChangesAsync();

            ToastMessage = $"Dodali ste {Amount:F2} € na račun.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostWithdrawAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                InlineError = "Uporabnik ni prijavljen.";
                await OnGetAsync();
                return Page();
            }

            if (Amount <= 0)
            {
                InlineError = "Prosim, vnesite veljaven znesek.";
                await OnGetAsync();
                return Page();
            }

            if (user.Balance < Amount)
            {
                InlineError = "Na računu ni dovolj sredstev.";
                await OnGetAsync();
                return Page();
            }

            user.Balance -= Amount;
            await _userManager.UpdateAsync(user);

            _db.Transactions.Add(new Transaction
            {
                UserId = user.Id,
                Amount = Amount,
                Date = DateTime.UtcNow,
                Type = "Withdraw",
                Description = $"Dvig {Amount:F2} €"
            });

            await _db.SaveChangesAsync();

            ToastMessage = $"Uspešno ste dvignili {Amount:F2} €.";
            return RedirectToPage();
        }
    }
}