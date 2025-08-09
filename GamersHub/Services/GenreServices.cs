using GamersHub.Data;
using GamersHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamersHub.Services
{
    /// <summary>
    /// Service class to handle CRUD operations for genres.
    /// </summary>
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext _context;

        public GenreService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all genres.
        /// </summary>
        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        /// <summary>
        /// Gets a genre by its ID or null if not found.
        /// </summary>
        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        /// <summary>
        /// Creates a new genre.
        /// </summary>
        public async Task CreateAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing genre.
        /// </summary>
        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a genre by its ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if a genre exists by ID.
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Genres.AnyAsync(g => g.Id == id);
        }
    }
}
