// Data/DbInitializer.cs
using Microsoft.AspNetCore.Identity;
using BetsiApp.Data;
using BetsiApp.Models;

namespace BetsiApp.SeedData
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. USTVARJANJE TESTNEGA UPORABNIKA (Admin/Test)
            if (userManager.FindByNameAsync("admin@betsi.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "admin@betsi.com",
                    Email = "admin@betsi.com",
                    EmailConfirmed = true 
                };

                // Geslo: Test123!
                IdentityResult result = await userManager.CreateAsync(user, "Test123!");

                if (result.Succeeded)
                {
                    Console.WriteLine("Testni uporabnik 'admin@betsi.com' uspešno ustvarjen.");
                }
            }
            
            // 2. USTVARJANJE DRUGEGA TESTNEGA UPORABNIKA
            if (userManager.FindByNameAsync("uporabnik@betsi.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "uporabnik@betsi.com",
                    Email = "uporabnik@betsi.com",
                    EmailConfirmed = true 
                };

                // Geslo: Uporabnik123!
                IdentityResult result = await userManager.CreateAsync(user, "Uporabnik123!");
            }
        }
    }
}