using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCMS.Infrastructure.Persistence.Configurations
{
    public class CaseConfiguration : IEntityTypeConfiguration<Case>
    {
        public void Configure(EntityTypeBuilder<Case> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CaseNumber).IsRequired().HasMaxLength(50);
            builder.HasIndex(c => c.CaseNumber).IsUnique();
            builder.Property(c => c.DefendantName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.DefendantAadhaar).HasMaxLength(12);
            builder.Property(c => c.DefendantPAN).HasMaxLength(10);
            builder.Property(c => c.DefendantAccountNumber).HasMaxLength(20);
            builder.Property(c => c.DefendantBankName).HasMaxLength(100);
            builder.Property(c => c.FreezeAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.MatchedBalance).HasColumnType("decimal(18,2)");
            
            builder.HasMany(c => c.Attachments)
                   .WithOne(a => a.Case)
                   .HasForeignKey(a => a.CaseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
