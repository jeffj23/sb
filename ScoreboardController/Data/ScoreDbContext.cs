using Microsoft.EntityFrameworkCore;

namespace ScoreboardController.Data
{
    public class ScoreDbContext : DbContext
    {
        public ScoreDbContext(DbContextOptions<ScoreDbContext> options)
            : base(options)
        {
        }

        // Replace with your actual table
        public DbSet<SoftKey> SoftKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entities, table names, etc.
        }
    }
}