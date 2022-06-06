using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Practice.Data.ContextConfigurations;
using Practice.Entities.Entities;
using System;

namespace Practice.Data
{
    public class DBContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ChallengeCommitConfig());
            modelBuilder.ApplyConfiguration(new ChallengeConfig());
            modelBuilder.ApplyConfiguration(new ImageConfig());
            modelBuilder.ApplyConfiguration(new UserChallengeConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new AchievementConfig());
            modelBuilder.ApplyConfiguration(new ChallengeTypeConfig());
            modelBuilder.ApplyConfiguration(new VenueConfig());

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<ChallengeCommit> ChallengeCommits { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }
        public DbSet<ChallengeType> ChallengeTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Venue> Venues { get; set; }
    }
}
