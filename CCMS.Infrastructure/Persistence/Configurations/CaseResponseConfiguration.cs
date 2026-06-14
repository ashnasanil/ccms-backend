using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCMS.Infrastructure.Persistence.Configurations
{
    public class CaseResponseConfiguration : IEntityTypeConfiguration<CaseResponse>
    {
        public void Configure(EntityTypeBuilder<CaseResponse> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.CaseId).IsUnique();
            builder.Property(r => r.FreezeAmount).HasColumnType("decimal(18,2)");
            builder.Property(r => r.BalanceAmount).HasColumnType("decimal(18,2)");
            
            builder.HasOne(r => r.Case)
                   .WithOne(c => c.CaseResponse)
                   .HasForeignKey<CaseResponse>(r => r.CaseId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
