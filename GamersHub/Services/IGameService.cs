using GamersHub.Models;

namespace GamersHub.Services
{
    /// <summary>
    /// Defines the contract for game-related business logic.
    /// Controllers will use this interface instead of directly
    /// working with ApplicationDbContext or Entity Framework queries.
    /// Benefits:
    /// - Improves testability (can mock IGameService in unit tests)
    /// - Keeps controllers focused on HTTP flow, not data access
    /// - Allows changing data access logic without touching controllers
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Retrieves a paginated list of games with optional search and genre filtering.
        /// Also returns the total number of pages for pagination controls.
        /// </summary>
        /// <param name="searchString">Optional title text to search for.</param>
        /// <param name="genreId">Optional genre ID to filter by. Pass null or 0 for all genres.</param>
        /// <param name="page">Page number (1-based index).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>
        /// A tuple containing:
        /// - Games: The filtered and paginated list of games.
        /// - TotalPages: Total number of pages given the current filter.
        /// </returns>
        Task<(IEnumerable<Game> Games, int TotalPages)> GetGamesAsync(
            string? searchString,
            int? genreId,
            int page,
            int pageSize
        );


        /// <summary>
        /// Retrieves a single game by its ID, or null if not found.
        /// </summary>
        Task<Game?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new game entry and optionally saves an uploaded image file.
        /// </summary>
        /// <param name="game">The game entity to create.</param>
        /// <param name="imageFile">Optional image file to store in wwwroot/images/games.</param>
        Task CreateAsync(Game game, IFormFile? imageFile);


        /// <summary>
        /// Updates an existing game entry by ID and optionally replaces its image file.
        /// </summary>
        /// <param name="id">The ID of the game to update.</param>
        /// <param name="game">The updated game data.</param>
        /// <param name="imageFile">Optional new image file. If provided, replaces the old one.</param>
        Task UpdateAsync(int id, Game game, IFormFile? imageFile);

        /// <summary>
        /// Deletes a game from the database by ID.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
