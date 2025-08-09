using GamersHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre?> GetByIdAsync(int id);
        Task CreateAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
