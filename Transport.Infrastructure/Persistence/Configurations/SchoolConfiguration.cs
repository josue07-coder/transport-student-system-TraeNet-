using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class SchoolConfiguration : IEntityTypeConfiguration<School>
    {
        public void Configure(EntityTypeBuilder<School> builder)
        {
            //  PK
            builder.HasKey(x => x.Id);

            //  Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            //  Director
            builder.Property(x => x.DirectorName)
                .IsRequired()
                .HasMaxLength(150);

            //  Sector
            builder.Property(x => x.SectorId)
                .IsRequired();

            builder.HasOne<Sector>()
                .WithMany(s => s.Schools)
                .HasForeignKey(x => x.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Description
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            //  Profile Image
            builder.Property(x => x.ProfileImageUrl)
                .HasMaxLength(300);

            //  Email (ValueObject)
            builder.OwnsOne(x => x.ContactEmail, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(150);
            });

            //  Phone (ValueObject)
            builder.OwnsOne(x => x.ContactPhone, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("Phone")
                    .IsRequired()
                    .HasMaxLength(50);
            });

            //  Address (ValueObject)
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
            builder.Property(x => x.IsActive)
                    .IsRequired();
        }
    }
}