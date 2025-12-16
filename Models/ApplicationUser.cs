// Models/ApplicationUser.cs

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BetsiApp.Models
{
    // Razširitev osnovnega IdentityUser modela z lastnimi lastnostmi
    public class ApplicationUser : IdentityUser
    {
        // Lastnost za shranjevanje stanja uporabnika
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; } = 1000.00m; // Privzeto stanje (npr. 1000)

        // Opomba: Tukaj lahko dodate še druge lastnosti, kot so ime, priimek itd.
    }
}