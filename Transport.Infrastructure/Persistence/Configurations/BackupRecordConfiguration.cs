using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class BackupRecordConfiguration : IEntityTypeConfiguration<BackupRecord>
    {
        public void Configure(EntityTypeBuilder<BackupRecord> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(260);

            builder.Property(x => x.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedByUserId)
                .IsRequired();

            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Type);
        }
    }
}
