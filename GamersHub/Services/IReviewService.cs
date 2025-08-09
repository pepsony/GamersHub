using System.Collections.Generic;
using System.Threading.Tasks;
using GamersHub.Models;

namespace GamersHub.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync(string? searchString, int? genreId);
        Task<Review?> GetReviewByIdAsync(int id);
        Task<bool> CreateReviewAsync(Review review, string userId);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int id);
        Task<bool> HasUserReviewedGameAsync(string userId, int gameId);
    }
}
