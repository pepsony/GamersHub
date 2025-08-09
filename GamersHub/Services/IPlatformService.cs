using GamersHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamersHub.Services
{
    /// <summary>
    /// Interface defining platform-related operations.
    /// </summary>
    public interface IPlatformService
    {
        Task<IEnumerable<Platform>> GetAllAsync();
        Task<Platform?> GetByIdAsync(int id);
        Task CreateAsync(Platform platform);
        Task UpdateAsync(Platform platform);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
