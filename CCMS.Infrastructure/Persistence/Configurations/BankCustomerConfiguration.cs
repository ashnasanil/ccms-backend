using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCMS.Infrastructure.Persistence.Configurations
{
    public class BankCustomerConfiguration : IEntityTypeConfiguration<BankCustomer>
    {
        public void Configure(EntityTypeBuilder<BankCustomer> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.AccountNumber).IsRequired().HasMaxLength(20);
            builder.HasIndex(b => b.AccountNumber).IsUnique();
            builder.Property(b => b.Aadhaar).HasMaxLength(12);
            builder.Property(b => b.PAN).HasMaxLength(10);
            builder.Property(b => b.Balance).HasColumnType("decimal(18,2)");
            builder.Property(b => b.BankName).IsRequired().HasMaxLength(100);
        }
    }
}
