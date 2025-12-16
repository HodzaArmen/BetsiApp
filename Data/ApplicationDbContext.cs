// Data/ApplicationDbContext.cs

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BetsiApp.Models; 

namespace BetsiApp.Data
{
    // SPREMENI IdentityUser v ApplicationUser
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    
        // ... (Ostali DbSet-i ostanejo enaki)
        public DbSet<Odd> Odds { get; set; }
        public DbSet<BetSlip> BetSlips { get; set; }
        public DbSet<BetItem> BetItems { get; set; }
    }
}