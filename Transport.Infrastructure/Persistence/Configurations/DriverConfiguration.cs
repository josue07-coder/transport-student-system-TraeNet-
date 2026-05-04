using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            //  Primary Key
            builder.HasKey(x => x.Id);

            //  FirstName
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            //  LastName
            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            //  LicenseNumber (ValueObject)
            builder.OwnsOne(x => x.LicenseNumber, license =>
            {
                license.Property(l => l.Value)
                    .HasColumnName("LicenseNumber")
                    .IsRequired()
                    .HasMaxLength(50);

                //  Índice único CORRECTO
                license.HasIndex(l => l.Value)
                    .IsUnique();
            });

            //  Forzar required del ValueObject
            builder.Navigation(x => x.LicenseNumber)
                .IsRequired();

            //  Photo
            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(300);

            //  Soft delete
            builder.Property(x => x.IsActive)
                .IsRequired();
        }
    }
}