using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
    {
        public void Configure(EntityTypeBuilder<Guardian> builder)
        {
            //  PK
            builder.HasKey(x => x.Id);

            //  Document
            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.DocumentType)
                .IsRequired()
                .HasConversion<string>();

            //  Unique document
            builder.HasIndex(x => x.DocumentNumber)
                .IsUnique();

            //  Names
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            //  Contact
            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(200);

            //  Photo
            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(300);

            //  Gender
            builder.Property(x => x.Gender)
                .IsRequired()
                .HasConversion<string>();

            //  Sector
            builder.Property(x => x.SectorId)
                .IsRequired();

            builder.HasOne<Sector>()
                .WithMany(s => s.Guardians)
                .HasForeignKey(x => x.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Guardian → Students
            builder.HasMany(x => x.Students)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.IsActive)
                 .IsRequired();
        }
    }
}