using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(notification => notification.Id);

            builder.Property(notification => notification.UserId)
                .IsRequired();

            builder.Property(notification => notification.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(notification => notification.Message)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(notification => notification.Type)
                .IsRequired();

            builder.Property(notification => notification.Priority)
                .IsRequired();

            builder.Property(notification => notification.IsRead)
                .IsRequired();

            builder.Property(notification => notification.RelatedEntityType)
                .HasMaxLength(150);

            builder.Property(notification => notification.RelatedEntityId)
                .HasMaxLength(100);

            builder.HasOne(notification => notification.User)
                .WithMany()
                .HasForeignKey(notification => notification.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(notification => notification.UserId);
            builder.HasIndex(notification => notification.IsRead);
            builder.HasIndex(notification => notification.CreatedAt);
            builder.HasIndex(notification => new { notification.UserId, notification.IsRead, notification.CreatedAt });
            builder.HasIndex(notification => new { notification.RelatedEntityType, notification.RelatedEntityId });
        }
    }
}
