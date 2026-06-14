using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Temporary for batch testing
    public DbSet<Case> Cases { get; set; }

    // Batch Module
    public DbSet<BankCustomer> BankCustomers { get; set; }

    public DbSet<BatchJobLog> BatchJobLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BankCustomer>()
            .Property(x => x.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Case>()
            .Property(x => x.MatchedBalance)
            .HasPrecision(18, 2);
    }
}