using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BetsiApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetsiApp.Data; 

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
        public string Message { get; set; } = string.Empty;

        [BindProperty]
        public decimal Amount { get; set; }

        // 🔹 Zgodovina iz baze
        public List<Transaction> History { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Balance = user?.Balance ?? 0.00m;

            if (user != null)
            {
                History = await _db.Transactions
                    .Where(t => t.UserId == user.Id)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAddMoneyAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && Amount > 0)
            {
                user.Balance += Amount;
                await _userManager.UpdateAsync(user);

                // shrani transakcijo v bazo
                var transaction = new Transaction
                {
                    UserId = user.Id,
                    Amount = Amount,
                    Date = DateTime.UtcNow,
                    Type = "Deposit",
                    Description = $"Polog {Amount:F2} €"
                };
                _db.Transactions.Add(transaction);
                await _db.SaveChangesAsync();

                Message = $"Dodali ste {Amount:F2} € na račun.";
            }
            else
            {
                Message = "Prosimo, vnesite veljaven znesek.";
            }

            await OnGetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostWithdrawAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && Amount > 0)
            {
                if (user.Balance >= Amount)
                {
                    user.Balance -= Amount;
                    await _userManager.UpdateAsync(user);

                    // shrani transakcijo v bazo
                    var transaction = new Transaction
                    {
                        UserId = user.Id,
                        Amount = Amount,
                        Date = DateTime.UtcNow,
                        Type = "Withdraw",
                        Description = $"Dvig {Amount:F2} €"
                    };
                    _db.Transactions.Add(transaction);
                    await _db.SaveChangesAsync();

                    Message = $"Uspešno ste dvignili {Amount:F2} €.";
                }
                else
                {
                    Message = "Na računu ni dovolj sredstev.";
                }
            }
            else
            {
                Message = "Prosimo, vnesite veljaven znesek.";
            }

            await OnGetAsync();
            return Page();
        }
    }
}