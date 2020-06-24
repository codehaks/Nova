using Microsoft.EntityFrameworkCore;

namespace Portal.RatingService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PostRating> PostRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PostRating>().HasKey(p => new { p.PostId, p.UserId });
            base.OnModelCreating(builder);
        }
    }
}