using GamersHub.Data;
using GamersHub.Models;
using Microsoft.EntityFrameworkCore;

namespace GamersHub.Services
{
    /// <summary>
    /// Service layer implementation for managing games.
    /// Handles all data access logic so controllers stay clean.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Main constructor for production use.
        /// Requires the database context and web host environment
        /// (for saving uploaded images to wwwroot).
        /// </summary>
        public GameService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Retrieves a filtered, paginated list of games.
        /// Supports optional search by title and filtering by genre.
        /// </summary>
        public async Task<(IEnumerable<Game> Games, int TotalPages)> GetGamesAsync(
            string? searchString,
            int? genreId,
            int page,
            int pageSize)
        {
            // Start query including Genre to avoid lazy loading
            var query = _context.Games
                                .Include(g => g.Genre)
                                .AsQueryable();

            // Filter by search term if provided
            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(g => g.Title.Contains(searchString));

            // Filter by genre if a valid ID is provided (> 0)
            if (genreId.HasValue && genreId.Value > 0)
                query = query.Where(g => g.GenreId == genreId);

            // Count total items for pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Fetch the current page
            var games = await query
                .OrderBy(g => g.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (games, totalPages);
        }

        /// <summary>
        /// Retrieves a single game by ID, including Genre.
        /// Returns null if not found.
        /// </summary>
        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games
                                 .Include(g => g.Genre)
                                 .FirstOrDefaultAsync(g => g.Id == id);
        }

        /// <summary>
        /// Creates a new game and optionally saves an image file to disk.
        /// </summary>
        public async Task CreateAsync(Game game, IFormFile? imageFile)
        {
            if (imageFile != null)
                game.ImagePath = await SaveImageAsync(imageFile);

            _context.Add(game);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing game by ID.
        /// If an image file is provided, replaces the old image.
        /// </summary>
        public async Task UpdateAsync(int id, Game updatedGame, IFormFile? imageFile)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return; // Exit if game not found

            // Update basic properties
            game.Title = updatedGame.Title;
            game.ReleaseDate = updatedGame.ReleaseDate;
            game.GenreId = updatedGame.GenreId;

            // Update image if provided
            if (imageFile != null)
                game.ImagePath = await SaveImageAsync(imageFile);

            _context.Update(game);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a game from the database by ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Saves an uploaded image file to wwwroot/images/games.
        /// Generates a unique filename to prevent overwriting.
        /// </summary>
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            var extension = Path.GetExtension(imageFile.FileName);
            var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

            var path = Path.Combine(_env.WebRootPath, "images/games", newFileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return relative path for use in views
            return $"/images/games/{newFileName}";
        }
    }
}
