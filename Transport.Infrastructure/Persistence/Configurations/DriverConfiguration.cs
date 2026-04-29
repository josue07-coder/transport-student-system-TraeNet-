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

            builder.HasKey(d => d.Id);

            // 🔹 Campos simples
            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(d => d.Email)
                .IsUnique();

            builder.Property(d => d.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.DocumentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(d => d.DocumentNumber)
                .IsUnique();

            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(500);

            // 🔹 Enums (se guardan como int por defecto)
            builder.Property(x => x.DocumentType)
                .HasConversion<string>() 
                .IsRequired();

            builder.Property(x => x.Gender)
                .HasConversion<string>()
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
            builder.HasMany(d => d.RouteAssignments)
                .WithOne(r => r.Driver)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Relación: Driver → VehicleAssignments (1:N)
            builder.HasMany(d => d.VehicleAssignments)
                .WithOne(v => v.Driver)
                .HasForeignKey(v => v.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
