using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class VehicleLocationConfiguration : IEntityTypeConfiguration<VehicleLocation>
    {
        public void Configure(EntityTypeBuilder<VehicleLocation> builder)
        {
            builder.HasKey(location => location.Id);

            builder.Property(location => location.Latitude).HasColumnType("decimal(9,6)").IsRequired();
            builder.Property(location => location.Longitude).HasColumnType("decimal(9,6)").IsRequired();
            builder.Property(location => location.Speed).HasColumnType("decimal(8,2)");
            builder.Property(location => location.Heading).HasColumnType("decimal(6,2)");
            builder.Property(location => location.RecordedAt).IsRequired();

            builder.HasOne(location => location.Trip)
                .WithMany()
                .HasForeignKey(location => location.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(location => location.Vehicle)
                .WithMany()
                .HasForeignKey(location => location.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(location => location.ReportedByUser)
                .WithMany()
                .HasForeignKey(location => location.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(location => location.TripId);
            builder.HasIndex(location => location.VehicleId);
            builder.HasIndex(location => location.RecordedAt);
            builder.HasIndex(location => new { location.TripId, location.RecordedAt });
        }
    }
}
