using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using GamersHub.Data;
using GamersHub.Models;
using GamersHub.Services;
using Moq; // For mocking IWebHostEnvironment
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Tests.Services
{
    /// <summary>
    /// Unit tests for GameService.
    /// Uses EF Core InMemory database and a mocked IWebHostEnvironment
    /// so no actual files are touched.
    /// </summary>
    [TestFixture]
    public class GameServiceTests
    {
        private ApplicationDbContext _context;
        private GameService _service;
        private Mock<IWebHostEnvironment> _mockEnv;

        /// <summary>
        /// Runs before each test.
        /// Sets up a new in-memory database and a fresh GameService.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Create a unique in-memory DB for isolation between tests
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);

            // Mock IWebHostEnvironment to avoid file system access
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.WebRootPath).Returns("fakepath"); // Simulate a path

            // Seed test data
            SeedTestData();

            // Pass mock object to GameService
            _service = new GameService(_context, _mockEnv.Object);
        }

        /// <summary>
        /// Runs after each test.
        /// Cleans up the in-memory database to free resources.
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        /// <summary>
        /// Adds predefined test data to the in-memory DB.
        /// </summary>
        private void SeedTestData()
        {
            _context.Games.AddRange(
                new Game { Id = 1, Title = "Halo", GenreId = 1 },
                new Game { Id = 2, Title = "Half-Life", GenreId = 2 }
            );
            _context.SaveChanges();
        }


        /// <summary>
        /// Ensures that all games are returned when no filters are applied.
        /// </summary>
        [Test]
        public async Task GetGamesAsync_ReturnsAllGames_WhenNoFilter()
        {
            // Act
            var (games, totalPages) = await _service.GetGamesAsync(null, null, page: 1, pageSize: 10);

            // Assert
            Assert.That(games.Count(), Is.EqualTo(2));
            Assert.That(totalPages, Is.EqualTo(1));
        }


        /// <summary>
        /// Ensures that a search by title returns only matching games.
        /// </summary>
        [Test]
        public async Task GetGamesAsync_FiltersByTitle()
        {
            // Act
            var (games, totalPages) = await _service.GetGamesAsync("Halo", null, page: 1, pageSize: 10);

            // Assert
            Assert.That(games.Count(), Is.EqualTo(1));
            Assert.That(games.First().Title, Is.EqualTo("Halo"));
        }

        /// <summary>
        /// Ensures that filtering by genre returns only games in that genre.
        /// </summary>
        [Test]
        public async Task GetGamesAsync_FiltersByGenre()
        {
            // Act
            var (games, totalPages) = await _service.GetGamesAsync(null, 1, page: 1, pageSize: 10);

            // Assert
            Assert.That(games.Count(), Is.EqualTo(1));
            Assert.That(games.First().Title, Is.EqualTo("Halo"));
        }

        [Test]
        public async Task GetGamesAsync_PaginatesResults()
        {
            // Arrange - add more games to test pagination
            for (int i = 3; i <= 15; i++)
            {
                _context.Games.Add(new Game { Id = i, Title = $"Game{i}", GenreId = 1 });
            }
            _context.SaveChanges();

            // Act - page size 5, get page 2
            var (games, totalPages) = await _service.GetGamesAsync(null, null, page: 2, pageSize: 5);

            // Assert
            Assert.That(games.Count(), Is.EqualTo(5)); // page size count
            Assert.That(totalPages, Is.EqualTo(3));    // total items 15 / 5 = 3 pages
        }
    }
}
