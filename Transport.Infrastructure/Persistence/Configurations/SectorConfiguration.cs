using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class SectorConfiguration : IEntityTypeConfiguration<Sector>
    {
        public void Configure(EntityTypeBuilder<Sector> builder)
        {
            //  Primary Key
            builder.HasKey(x => x.Id);

            //  Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            //  City
            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            //  Province
            builder.Property(x => x.Province)
                .IsRequired()
                .HasMaxLength(100);

            //  SchoolDistrict
            builder.Property(x => x.SchoolDistrictId)
                .IsRequired(false);

            builder.HasOne(x => x.SchoolDistrict)
                .WithMany(x => x.Sectors)
                .HasForeignKey(x => x.SchoolDistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Sector → Schools
            builder.HasMany(x => x.Schools)
                .WithOne()
                .HasForeignKey("SectorId")
                .OnDelete(DeleteBehavior.Restrict);

            //  Sector → Stops
            builder.HasMany(x => x.Stops)
                .WithOne()
                .HasForeignKey("SectorId")
                .OnDelete(DeleteBehavior.Restrict);

            //  Sector → Guardians
            builder.HasMany(x => x.Guardians)
                .WithOne(x => x.Sector)
                .HasForeignKey(x => x.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Índice útil (opcional pero pro)
            builder.HasIndex(x => new { x.Name, x.City })
                .IsUnique(false);
        }
    }
}
