using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class TripRouteDeviationConfiguration : IEntityTypeConfiguration<TripRouteDeviation>
    {
        public void Configure(EntityTypeBuilder<TripRouteDeviation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReasonType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Reason)
                .HasMaxLength(300);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.Latitude)
                .HasColumnType("decimal(9,6)")
                .IsRequired(false);

            builder.Property(x => x.Longitude)
                .HasColumnType("decimal(9,6)")
                .IsRequired(false);

            builder.Property(x => x.ReportedAt)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Trip)
                .WithMany(x => x.RouteDeviations)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReportedByUser)
                .WithMany()
                .HasForeignKey(x => x.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TripId);
            builder.HasIndex(x => x.ReportedByUserId);
            builder.HasIndex(x => x.ReportedAt);
        }
    }
}
