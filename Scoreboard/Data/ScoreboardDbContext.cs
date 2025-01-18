using Microsoft.EntityFrameworkCore;
using Scoreboard.Models;

namespace Scoreboard.Data
{
    public class ScoreDbContext : DbContext
    {
        public ScoreDbContext(DbContextOptions<ScoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<ScoreboardSetModel> ScoreboardSets { get; set; }
        public DbSet<ScoreboardElementModel> ScoreboardElements { get; set; }
        public DbSet<ScoreboardSetElementModel> ScoreboardSetElements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ScoreboardSetModel>().HasKey(s => s.SetId);

            // ScoreboardSetElements primary key: (SetId, ElementId)
            modelBuilder.Entity<ScoreboardSetElementModel>()
                .HasKey(se => new { se.SetId, se.ElementId });

            // Relationships:
            modelBuilder.Entity<ScoreboardSetElementModel>()
                .HasOne(se => se.Set)
                .WithMany(s => s.SetElements)
                .HasForeignKey(se => se.SetId);

            modelBuilder.Entity<ScoreboardSetElementModel>()
                .HasOne(se => se.Element)
                .WithMany(e => e.SetElements)
                .HasForeignKey(se => se.ElementId);
        }
    }
}