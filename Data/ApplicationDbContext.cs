// Data/ApplicationDbContext.cs

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BetsiApp.Models; // Uvozite nove modele

namespace BetsiApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    
        // DODAJ TUKAJ: Modeli za stave
        public DbSet<Odd> Odds { get; set; }
        public DbSet<BetSlip> BetSlips { get; set; }
        public DbSet<BetItem> BetItems { get; set; }
    }
}