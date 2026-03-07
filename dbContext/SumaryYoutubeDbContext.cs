using Microsoft.EntityFrameworkCore;
using SumaryYoutubeBackend.Models;

namespace SumaryYoutubeBackend.dbContext
{

    public class SumaryYoutubeDbContext : DbContext
    {
        public SumaryYoutubeDbContext(DbContextOptions<SumaryYoutubeDbContext> options)
            : base(options)
        {
        }

        public DbSet<VideoSummary> VideoSummaries { get; set; }
    }
}