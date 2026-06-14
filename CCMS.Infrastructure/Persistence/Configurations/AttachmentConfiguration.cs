using CCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCMS.Infrastructure.Persistence.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.FileName).IsRequired().HasMaxLength(255);
            builder.Property(a => a.FileType).HasMaxLength(50);
            builder.Property(a => a.StoragePath).IsRequired().HasMaxLength(500);
        }
    }
}
