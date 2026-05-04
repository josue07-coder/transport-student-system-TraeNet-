using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class StopConfiguration : IEntityTypeConfiguration<Stop>
    {
        public void Configure(EntityTypeBuilder<Stop> builder)
        {
            //  PK
            builder.HasKey(x => x.Id);

            //  Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            //  Sector
            builder.Property(x => x.SectorId)
                .IsRequired();

            builder.HasOne<Sector>()
                .WithMany(s => s.Stops)
                .HasForeignKey(x => x.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Address (ValueObject AJUSTADO)
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

            //  Coordinates
            builder.OwnsOne(x => x.Coordinates, coord =>
            {
                coord.Property(c => c.Latitude)
                    .HasColumnName("Latitude")
                    .IsRequired();

                coord.Property(c => c.Longitude)
                    .HasColumnName("Longitude")
                    .IsRequired();
            });
        }
    }
}