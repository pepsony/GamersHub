using System;
using System.Linq;
using System.Threading.Tasks;
using GamersHub.Data;
using GamersHub.Models;
using GamersHub.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests.Services
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private ApplicationDbContext _context = null!;
        private ReviewService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);

            SeedTestData();

            _service = new ReviewService(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedTestData()
        {
            // Seed games, genres, users and reviews
            var genre = new Genre { Id = 1, Name = "Action" };
            var game = new Game { Id = 1, Title = "Halo", GenreId = 1, Genre = genre };
            var user = new ApplicationUser { Id = "user1", UserName = "testuser" };

            _context.Genres.Add(genre);
            _context.Games.Add(game);
            _context.Users.Add(user);

            _context.Reviews.Add(new Review
            {
                Id = 1,
                GameId = 1,
                Game = game,
                UserId = "user1",
                User = user,
                Content = "Great game!",
                Rating = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });

            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllReviewsAsync_ReturnsAllReviews()
        {
            var reviews = await _service.GetAllReviewsAsync(null, null);
            Assert.That(reviews.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetReviewByIdAsync_ReturnsCorrectReview()
        {
            var review = await _service.GetReviewByIdAsync(1);
            Assert.IsNotNull(review);
            Assert.That(review!.Content, Is.EqualTo("Great game!"));
        }

        [Test]
        public async Task HasUserReviewedGameAsync_ReturnsTrueIfReviewed()
        {
            bool hasReviewed = await _service.HasUserReviewedGameAsync("user1", 1);
            Assert.IsTrue(hasReviewed);
        }

        [Test]
        public async Task CreateReviewAsync_ReturnsFalseIfAlreadyReviewed()
        {
            var newReview = new Review
            {
                GameId = 1,
                Content = "Another review",
                Rating = 4
            };

            bool created = await _service.CreateReviewAsync(newReview, "user1");
            Assert.IsFalse(created);
        }

        [Test]
        public async Task CreateReviewAsync_ReturnsTrueIfNewReview()
        {
            var newReview = new Review
            {
                GameId = 1,
                Content = "New review",
                Rating = 4
            };

            bool created = await _service.CreateReviewAsync(newReview, "newuser");
            Assert.IsTrue(created);
        }

        [Test]
        public async Task UpdateReviewAsync_UpdatesReview()
        {
            var reviewToUpdate = await _service.GetReviewByIdAsync(1);
            reviewToUpdate!.Content = "Updated content";

            bool updated = await _service.UpdateReviewAsync(reviewToUpdate);
            Assert.IsTrue(updated);

            var updatedReview = await _service.GetReviewByIdAsync(1);
            Assert.That(updatedReview!.Content, Is.EqualTo("Updated content"));
        }

        [Test]
        public async Task DeleteReviewAsync_RemovesReview()
        {
            bool deleted = await _service.DeleteReviewAsync(1);
            Assert.IsTrue(deleted);

            var review = await _service.GetReviewByIdAsync(1);
            Assert.IsNull(review);
        }
    }
}
