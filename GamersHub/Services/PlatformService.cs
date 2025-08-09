using GamersHub.Data;
using GamersHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamersHub.Services
{
    /// <summary>
    /// Service class to handle CRUD operations for platforms.
    /// </summary>
    public class PlatformService : IPlatformService
    {
        private readonly ApplicationDbContext _context;

        public PlatformService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all platforms.
        /// </summary>
        public async Task<IEnumerable<Platform>> GetAllAsync()
        {
            return await _context.Platforms.ToListAsync();
        }

        /// <summary>
        /// Gets a platform by its ID or null if not found.
        /// </summary>
        public async Task<Platform?> GetByIdAsync(int id)
        {
            return await _context.Platforms.FindAsync(id);
        }

        /// <summary>
        /// Creates a new platform.
        /// </summary>
        public async Task CreateAsync(Platform platform)
        {
            _context.Platforms.Add(platform);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing platform.
        /// </summary>
        public async Task UpdateAsync(Platform platform)
        {
            _context.Platforms.Update(platform);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a platform by its ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var platform = await _context.Platforms.FindAsync(id);
            if (platform != null)
            {
                _context.Platforms.Remove(platform);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if a platform exists by ID.
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Platforms.AnyAsync(p => p.Id == id);
        }
    }
}
