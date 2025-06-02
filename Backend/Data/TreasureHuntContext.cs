using Microsoft.EntityFrameworkCore;
using TreasureHuntApi.Models;

namespace TreasureHuntApi.Data
{
    public class TreasureHuntContext : DbContext
    {
        public TreasureHuntContext(DbContextOptions<TreasureHuntContext> options) : base(options)
        {
        }

        public DbSet<TreasureMatrix> TreasureMatrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreasureMatrix>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MatrixData).IsRequired();
                entity.Property(e => e.N).IsRequired();
                entity.Property(e => e.M).IsRequired();
                entity.Property(e => e.P).IsRequired();
                entity.Property(e => e.Result).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
} 