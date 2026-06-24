using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                .HasMaxLength(100);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.EntityName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.EntityId)
                .HasMaxLength(100);

            builder.Property(x => x.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NewValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(100);

            builder.Property(x => x.TraceId)
                .HasMaxLength(100);

            builder.Property(x => x.RequestPath)
                .HasMaxLength(300);

            builder.Property(x => x.HttpMethod)
                .HasMaxLength(20);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.EntityName, x.EntityId });
            builder.HasIndex(x => new { x.UserId, x.CreatedAt });
            builder.HasIndex(x => x.CorrelationId);
            builder.HasIndex(x => x.Action);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => new { x.Action, x.CreatedAt });
        }
    }
}
