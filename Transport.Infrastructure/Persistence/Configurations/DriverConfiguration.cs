using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DocumentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.DocumentNumber)
                .IsUnique();

            builder.OwnsOne(x => x.LicenseNumber, license =>
            {
                license.Property(l => l.Value)
                    .HasColumnName("LicenseNumber")
                    .IsRequired()
                    .HasMaxLength(50);

                license.HasIndex(l => l.Value)
                    .IsUnique();
            });

            builder.Navigation(x => x.LicenseNumber)
                .IsRequired();

            builder.OwnsOne(x => x.Phone, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("Phone")
                    .IsRequired()
                    .HasMaxLength(50);
            });

            builder.Navigation(x => x.Phone)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasConversion(
                    email => email == null ? null : email.Value,
                    value => string.IsNullOrWhiteSpace(value) ? null : Transport.Domain.ValueObjects.Email.Create(value))
                .HasColumnName("Email")
                .IsRequired(false)
                .HasMaxLength(150);

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("Street")
                    .IsRequired()
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("City")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.Navigation(x => x.Address)
                .IsRequired();

            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(300);

            builder.Property(x => x.IsActive)
                .IsRequired();
        }
    }
}
