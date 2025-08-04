using System;
using System.Linq;
using System.Threading.Tasks;
using GamersHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamersHub.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Seed roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed default admin user
            var adminEmail = "admin@abv.bg";
            var adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Seed Genres
            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "RPG" },
                    new Genre { Name = "Action" },
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "Shooter" },
                    new Genre { Name = "Simulation" },
                    new Genre { Name = "Indie" },
                    new Genre { Name = "Other" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Platforms
            if (!context.Platforms.Any())
            {
                context.Platforms.AddRange(
                    new Platform { Name = "PC" },
                    new Platform { Name = "PlayStation 5" },
                    new Platform { Name = "Xbox Series X" },
                    new Platform { Name = "Nintendo Switch" },
                    new Platform { Name = "Other" }
                );
                await context.SaveChangesAsync();
            }

            // Seed a demo game
            if (!context.Games.Any())
            {
                var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "RPG");

                if (genre != null)
                {
                    var game = new Game
                    {
                        Title = "Elder Scrolls VI",
                        GenreId = genre.Id,
                        ReleaseDate = new DateTime(2026, 1, 1)
                    };

                    context.Games.Add(game);
                    await context.SaveChangesAsync();

                    var platforms = await context.Platforms.ToListAsync();
                    foreach (var p in platforms)
                    {
                        context.GamePlatforms.Add(new GamePlatform
                        {
                            GameId = (int)game.Id,
                            PlatformId = p.Id
                        });
                    }

                    await context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Genre 'RPG' not found — skipping game seeding.");
                }
            }
        }
    }
}
