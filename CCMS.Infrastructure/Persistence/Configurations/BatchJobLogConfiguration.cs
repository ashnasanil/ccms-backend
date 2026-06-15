using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCMS.Infrastructure.Persistence.Configurations
{
    public class BatchJobLogConfiguration : IEntityTypeConfiguration<BatchJobLog>
    {
        public void Configure(EntityTypeBuilder<BatchJobLog> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Status).HasMaxLength(50);
        }
    }
}
