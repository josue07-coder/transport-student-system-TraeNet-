using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Key)
                .IsUnique();

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DataType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IsEditable)
                .IsRequired();
        }
    }
}
