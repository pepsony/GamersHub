using System;
using GamersHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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


            // Seed Genres
            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "RPG" },
                    new Genre { Name = "Action" },
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "Shooter" },
                    new Genre { Name = "Simulation" },
                    new Genre { Name = "Indie" }
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
                    new Platform { Name = "Nintendo Switch" }
                );
            }


            // Optional: Seed a demo game
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

                    // Add platforms for the game
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
