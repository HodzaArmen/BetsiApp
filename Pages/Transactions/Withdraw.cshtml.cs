using BetsiApp.Data;
using BetsiApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BetsiApp.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class WithdrawModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [Range(1, 10000, ErrorMessage = "Vnesite znesek med 1 in 10000.")]
            [DataType(DataType.Currency)]
            [Display(Name = "Znesek izplačila")]
            public decimal Amount { get; set; }
        }

        public WithdrawModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public void OnGet()
        {
        }

        // Metoda se izvede ob oddaji formularja (HTTP POST)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Napaka: Ni mogoče naložiti uporabnika z ID-jem '{_userManager.GetUserId(User)}'.");
            }
            
            // --- KLJUČNA VALIDACIJA: PREVERJANJE STANJA ---
            if (user.Balance < Input.Amount)
            {
                // Dodajanje napake v ModelState, ki se prikaže v Summary ali poleg polja
                ModelState.AddModelError(string.Empty, $"Napaka: Nimete dovolj sredstev. Trenutno stanje je {user.Balance:F2} €.");
                return Page();
            }

            // 2. Transakcijska logika: Zmanjšaj stanje
            try
            {
                user.Balance -= Input.Amount;
                
                // 3. Posodobi podatke v bazi
                await _context.SaveChangesAsync(); 

                StatusMessage = $"Uspešno ste izplačali {Input.Amount:F2} €! Vaše novo stanje je {user.Balance:F2} €.";
                return RedirectToPage("/Index"); 
            }
            catch (DbUpdateConcurrencyException)
            {
                StatusMessage = "Napaka: Prišlo je do napake pri shranjevanju transakcije. Poskusite znova.";
                return Page();
            }
        }
    }
}