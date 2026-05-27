using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class IncidentCommentConfiguration : IEntityTypeConfiguration<IncidentComment>
    {
        public void Configure(EntityTypeBuilder<IncidentComment> builder)
        {
            builder.HasKey(comment => comment.Id);

            builder.Property(comment => comment.Comment)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne(comment => comment.User)
                .WithMany()
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(comment => comment.IncidentId);
            builder.HasIndex(comment => comment.UserId);
            builder.HasIndex(comment => comment.CreatedAt);
        }
    }
}
