using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamersHub.Data;
using GamersHub.Models;
using Microsoft.EntityFrameworkCore;

namespace GamersHub.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fetch all reviews with optional filters for search and genre
        public async Task<IEnumerable<Review>> GetAllReviewsAsync(string? searchString, int? genreId)
        {
            var query = _context.Reviews
                .Include(r => r.Game)
                    .ThenInclude(g => g.Genre)
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(r => r.Game.Title.Contains(searchString));
            }

            if (genreId.HasValue && genreId != 0)
            {
                query = query.Where(r => r.Game.GenreId == genreId);
            }

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        // Get a single review by Id
        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Check if user already reviewed a specific game
        public async Task<bool> HasUserReviewedGameAsync(string userId, int gameId)
        {
            return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.GameId == gameId);
        }

        // Create a new review
        public async Task<bool> CreateReviewAsync(Review review, string userId)
        {
            if (await HasUserReviewedGameAsync(userId, review.GameId))
            {
                return false; // user already reviewed
            }

            review.UserId = userId;
            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            return (await _context.SaveChangesAsync()) > 0;
        }

        // Update an existing review
        public async Task<bool> UpdateReviewAsync(Review review)
        {
            var existingReview = await _context.Reviews.FindAsync(review.Id);
            if (existingReview == null)
            {
                return false;
            }

            existingReview.Content = review.Content;
            existingReview.Rating = review.Rating;
            existingReview.CreatedAt = review.CreatedAt;
            existingReview.GameId = review.GameId;
            existingReview.UserId = review.UserId;

            _context.Reviews.Update(existingReview);

            return (await _context.SaveChangesAsync()) > 0;
        }

        // Delete a review by Id
        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
