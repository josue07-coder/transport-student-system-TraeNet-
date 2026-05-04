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
                .IsRequired();

            builder.HasOne<SchoolDistrict>()
                .WithMany(d => d.Sectors)
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

            //  Índice útil (opcional pero pro)
            builder.HasIndex(x => new { x.Name, x.City })
                .IsUnique(false);
        }
    }
}