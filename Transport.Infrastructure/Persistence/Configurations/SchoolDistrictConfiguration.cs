using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class SchoolDistrictConfiguration : IEntityTypeConfiguration<SchoolDistrict>
    {
        public void Configure(EntityTypeBuilder<SchoolDistrict> builder)
        {
            //  Primary Key
            builder.HasKey(x => x.Id);

            //  Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            //  Code
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            //  Código único (muy importante)
            builder.HasIndex(x => x.Code)
                .IsUnique();

            //  Description (opcional)
            builder.Property(x => x.Description)
                .HasMaxLength(500);

            //  SchoolDistrict → Sectors
            builder.HasMany(x => x.Sectors)
                .WithOne(x => x.SchoolDistrict)
                .HasForeignKey(x => x.SchoolDistrictId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}
