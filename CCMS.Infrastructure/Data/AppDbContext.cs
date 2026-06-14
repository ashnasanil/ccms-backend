using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Case> Cases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Case entity
            modelBuilder.Entity<Case>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ComplainantName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DefendantName).IsRequired().HasMaxLength(100);
                
                // Keep decimal precision safe
                entity.Property(e => e.FreezeAmount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
