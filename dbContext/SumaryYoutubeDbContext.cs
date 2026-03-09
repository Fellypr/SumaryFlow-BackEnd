using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SumaryYoutubeBackend.Models;
using System.Linq;

namespace SumaryYoutubeBackend.dbContext
{

    public class SumaryYoutubeDbContext : DbContext
    {
        public SumaryYoutubeDbContext(DbContextOptions<SumaryYoutubeDbContext> options)
            : base(options)
        {
        }

        public DbSet<VideoSummary> VideoSummaries { get; set; }
        public DbSet<AuthUser> AuthUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoSummary>()
                .HasOne(t => t.AuthUser)
                .WithMany(u => u.VideosUser)
                .HasForeignKey(c => c.IdUser)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}