using GamersHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GamersHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        
            public DbSet<Game> Games { get; set; }
            public DbSet<Genre> Genres { get; set; }
            public DbSet<Platform> Platforms { get; set; }
            public DbSet<GamePlatform> GamePlatforms { get; set; }
            public DbSet<UserGame> UserGames { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<Follower> Followers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // GamePlatform (many-to-many)
            builder.Entity<GamePlatform>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.GamePlatforms)
                .HasForeignKey(gp => gp.GameId);

            builder.Entity<GamePlatform>()
                .HasOne(gp => gp.Platform)
                .WithMany()
                .HasForeignKey(gp => gp.PlatformId);

            // Follower relationships: FollowerUser follows FollowingUser
            builder.Entity<Follower>()
                .HasOne(f => f.FollowerUser)
                .WithMany()
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follower>()
                .HasOne(f => f.FollowingUser)
                .WithMany()
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Followers)
                .WithOne(f => f.FollowingUser)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Following)
                .WithOne(f => f.FollowerUser)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }    
}
