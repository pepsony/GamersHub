using System;
using System.Linq;
using System.Threading.Tasks;
using GamersHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

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

            
            // Helper - method to seed a game
            
            async Task SeedGameAsync(string title, string genreName, DateTime releaseDate)
            {
                if (!await context.Games.AnyAsync(g => g.Title == title))
                {
                    var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                    if (genre == null)
                    {
                        Console.WriteLine($"Genre '{genreName}' not found — skipping '{title}'.");
                        return;
                    }

                    var game = new Game
                    {
                        Title = title,
                        GenreId = genre.Id,
                        ReleaseDate = releaseDate
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

                    Console.WriteLine($"Seeded game: {title}");
                }
            }

            
            // Seed 15 Games
            
            await SeedGameAsync("Elder Scrolls VI", "RPG", new DateTime(2026, 1, 1));
            await SeedGameAsync("Cyberpunk 2078", "RPG", new DateTime(2028, 5, 10));
            await SeedGameAsync("Halo Infinite 2", "Shooter", new DateTime(2027, 9, 20));
            await SeedGameAsync("Mass Effect 5", "RPG", new DateTime(2027, 3, 15));
            await SeedGameAsync("The Witcher 4", "RPG", new DateTime(2026, 11, 5));
            await SeedGameAsync("Half-Life 3", "Shooter", new DateTime(2025, 12, 1));
            await SeedGameAsync("GTA VI", "Action", new DateTime(2025, 10, 15));
            await SeedGameAsync("Red Dead Redemption 3", "Action", new DateTime(2029, 4, 18));
            await SeedGameAsync("Metroid Prime 5", "Adventure", new DateTime(2026, 6, 30));
            await SeedGameAsync("Zelda: Breath of the Wild 3", "Adventure", new DateTime(2028, 9, 1));
            await SeedGameAsync("Forza Horizon 6", "Simulation", new DateTime(2025, 11, 22));
            await SeedGameAsync("Gran Turismo 8", "Simulation", new DateTime(2026, 7, 15));
            await SeedGameAsync("Stardew Valley 2", "Indie", new DateTime(2025, 8, 5));
            await SeedGameAsync("Hades II", "Indie", new DateTime(2025, 12, 25));
            await SeedGameAsync("Starfield 2", "RPG", new DateTime(2029, 1, 10));
        }
    }
}
