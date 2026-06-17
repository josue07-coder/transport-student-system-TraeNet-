using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class TripScheduleConfiguration : IEntityTypeConfiguration<TripSchedule>
    {
        public void Configure(EntityTypeBuilder<TripSchedule> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Direction)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(x => x.DepartureTime)
                .IsRequired()
                .HasColumnType("time");

            builder.Property(x => x.ArrivalTime)
                .HasColumnType("time")
                .IsRequired(false);

            builder.Property(x => x.ValidFrom)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.ValidTo)
                .HasColumnType("date")
                .IsRequired(false);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.Monday).IsRequired();
            builder.Property(x => x.Tuesday).IsRequired();
            builder.Property(x => x.Wednesday).IsRequired();
            builder.Property(x => x.Thursday).IsRequired();
            builder.Property(x => x.Friday).IsRequired();
            builder.Property(x => x.Saturday).IsRequired();
            builder.Property(x => x.Sunday).IsRequired();

            builder.HasOne(x => x.RouteAssignment)
                .WithMany()
                .HasForeignKey(x => x.RouteAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Trips)
                .WithOne(x => x.TripSchedule)
                .HasForeignKey(x => x.TripScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.RouteAssignmentId);
            builder.HasIndex(x => new { x.RouteAssignmentId, x.Direction, x.ValidFrom });
        }
    }
}
