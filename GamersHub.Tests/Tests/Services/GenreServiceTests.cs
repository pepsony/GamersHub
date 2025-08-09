using NUnit.Framework;
using GamersHub.Data;
using GamersHub.Models;
using GamersHub.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Tests.Services
{
    [TestFixture]
    public class GenreServiceTests
    {
        private ApplicationDbContext _context;
        private GenreService _service;

        [SetUp]
        public void Setup()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new GenreService(_context);

            SeedTestData();
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedTestData()
        {
            _context.Genres.AddRange(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Adventure" }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllGenres()
        {
            var genres = await _service.GetAllAsync();

            Assert.That(genres.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsGenre_WhenExists()
        {
            var genre = await _service.GetByIdAsync(1);

            Assert.That(genre, Is.Not.Null);
            Assert.That(genre!.Name, Is.EqualTo("Action"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            var genre = await _service.GetByIdAsync(999);

            Assert.That(genre, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsGenre()
        {
            var newGenre = new Genre { Name = "RPG" };
            await _service.CreateAsync(newGenre);

            var genres = await _service.GetAllAsync();
            Assert.That(genres.Any(g => g.Name == "RPG"), Is.True);
        }

        [Test]
        public async Task UpdateAsync_ChangesGenre()
        {
            var genre = await _service.GetByIdAsync(1);
            genre!.Name = "Action Updated";

            await _service.UpdateAsync(genre);

            var updatedGenre = await _service.GetByIdAsync(1);
            Assert.That(updatedGenre!.Name, Is.EqualTo("Action Updated"));
        }

        [Test]
        public async Task DeleteAsync_RemovesGenre()
        {
            await _service.DeleteAsync(1);

            var genre = await _service.GetByIdAsync(1);
            Assert.That(genre, Is.Null);
        }

        [Test]
        public async Task ExistsAsync_ReturnsTrue_WhenExists()
        {
            var exists = await _service.ExistsAsync(1);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ReturnsFalse_WhenNotExists()
        {
            var exists = await _service.ExistsAsync(999);
            Assert.That(exists, Is.False);
        }
    }
}
