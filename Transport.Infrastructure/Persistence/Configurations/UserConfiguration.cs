using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired().
                HasMaxLength(300);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.Password)
                .IsRequired();
               
            builder.Property(u => u.ProfileImageUrl)
                .HasMaxLength(500);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RolId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
