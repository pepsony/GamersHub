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
    public class PlatformServiceTests
    {
        private ApplicationDbContext _context;
        private PlatformService _service;

        [SetUp]
        public void Setup()
        {
            // Setup a fresh in-memory database for each test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new PlatformService(_context);

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
            _context.Platforms.AddRange(
                new Platform { Id = 1, Name = "PC" },
                new Platform { Id = 2, Name = "Xbox" }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllPlatforms()
        {
            var platforms = await _service.GetAllAsync();
            Assert.That(platforms.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsPlatform_WhenExists()
        {
            var platform = await _service.GetByIdAsync(1);
            Assert.That(platform, Is.Not.Null);
            Assert.That(platform!.Name, Is.EqualTo("PC"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            var platform = await _service.GetByIdAsync(999);
            Assert.That(platform, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsPlatform()
        {
            var newPlatform = new Platform { Name = "PlayStation" };
            await _service.CreateAsync(newPlatform);

            var platforms = await _service.GetAllAsync();
            Assert.That(platforms.Any(p => p.Name == "PlayStation"), Is.True);
        }

        [Test]
        public async Task UpdateAsync_ChangesPlatform()
        {
            var platform = await _service.GetByIdAsync(1);
            platform!.Name = "PC Updated";

            await _service.UpdateAsync(platform);

            var updatedPlatform = await _service.GetByIdAsync(1);
            Assert.That(updatedPlatform!.Name, Is.EqualTo("PC Updated"));
        }

        [Test]
        public async Task DeleteAsync_RemovesPlatform()
        {
            await _service.DeleteAsync(1);

            var platform = await _service.GetByIdAsync(1);
            Assert.That(platform, Is.Null);
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
