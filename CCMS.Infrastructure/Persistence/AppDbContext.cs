using Microsoft.EntityFrameworkCore;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Domain.Common;

namespace CCMS.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Case> Cases { get; set; } = null!;
        public DbSet<CaseResponse> CaseResponses { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BankCustomer> BankCustomers { get; set; } = null!;
        public DbSet<BatchJobLog> BatchJobLogs { get; set; } = null!;
        public DbSet<Attachment> Attachments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<BankCustomer>()
                .Property(x => x.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Case>()
                .Property(x => x.FreezeAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Case>()
                .Property(x => x.MatchedBalance)
                .HasPrecision(18, 2);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
