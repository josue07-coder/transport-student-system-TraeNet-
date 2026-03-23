using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration: IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("drivers");

            builder.HasKey(x => x.Id);

            // 🔹 Campos simples
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(500);

            // 🔹 Enums (se guardan como int por defecto)
            builder.Property(x => x.DocumentType)
                .IsRequired();

            builder.Property(x => x.Gender)
                .IsRequired();

            // 🔹 ValueObject: LicenseNumber
            builder.OwnsOne(x => x.LicenseNumber, license =>
            {
                license.Property(l => l.Value)
                    .HasColumnName("license_number")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            // 🔹 ValueObject: Address
            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("street")
                    .HasMaxLength(200);

                address.Property(a => a.Sector)
                    .HasColumnName("sector")
                    .HasMaxLength(100);

                address.Property(a => a.City)
                    .HasColumnName("city")
                    .HasMaxLength(100);
            });

            // 🔹 Relación: Driver → RouteAssignments (1:N)
            builder.HasMany(x => x.RouteAssignments)
                .WithOne(r => r.Driver)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Relación: Driver → VehicleAssignments (1:N)
            builder.HasMany(x => x.vehicleAssignments)
                .WithOne(v => v.Driver)
                .HasForeignKey(v => v.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
