using BetsiApp.Data;
using BetsiApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BetsiApp.Pages.Transactions
{
    // Potrebujemo Authorize atribut, da le prijavljeni uporabniki lahko dostopajo
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DepositModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        // Vhodni model za vnos vplačila
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [Range(1, 10000, ErrorMessage = "Vnesite znesek med 1 in 10000.")]
            [DataType(DataType.Currency)]
            [Display(Name = "Znesek vplačila")]
            public decimal Amount { get; set; }
        }

        public DepositModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Metoda se izvede pri prvem obisku strani (HTTP GET)
        public void OnGet()
        {
            // Na tej strani ne potrebujemo posebne logike pri GET, saj je samo formular.
        }

        // Metoda se izvede ob oddaji formularja (HTTP POST)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Če validacija ne uspe, se vrne stran s sporočili o napaki
            }

            // 1. Poišči trenutnega uporabnika
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Če uporabnik ni najden, čeprav je prijavljen, preusmeri
                return NotFound($"Napaka: Ni mogoče naložiti uporabnika z ID-jem '{_userManager.GetUserId(User)}'.");
            }
            
            // 2. Transakcijska logika: Povečaj stanje
            try
            {
                user.Balance += Input.Amount;
                
                // 3. Posodobi podatke v bazi
                await _context.SaveChangesAsync(); 

                StatusMessage = $"Uspešno ste vplačali {Input.Amount:F2} €! Vaše novo stanje je {user.Balance:F2} €.";
                return RedirectToPage("/Index"); // Preusmeri na začetno stran po uspešni transakciji
            }
            catch (DbUpdateConcurrencyException)
            {
                // Obravnava morebitne napake pri sočasnem dostopu do baze
                StatusMessage = "Napaka: Prišlo je do napake pri shranjevanju transakcije. Poskusite znova.";
                return Page();
            }
        }
    }
}