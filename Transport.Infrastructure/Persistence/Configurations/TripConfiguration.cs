using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            //  Primary Key
            builder.HasKey(x => x.Id);

            // Relación con RouteAssignment
            builder.HasOne(x => x.RouteAssignment)
                .WithMany(r => r.Trips)
                .HasForeignKey(x => x.RouteAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TripSchedule)
                .WithMany(x => x.Trips)
                .HasForeignKey(x => x.TripScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.StudentAttendances)
                .WithOne(x => x.Trip)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.RouteDeviations)
                .WithOne(x => x.Trip)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Direction)
                .HasConversion<string>()
                .IsRequired(false)
                .HasMaxLength(30);

            builder.Property(x => x.OperationDate)
                .HasColumnType("date")
                .IsRequired(false);

            builder.Property(x => x.ScheduledDepartureTime)
                .IsRequired(false);

            builder.Property(x => x.ScheduledArrivalTime)
                .IsRequired(false);

            //  Campos de tiempo
            builder.Property(x => x.StartTime)
                .IsRequired(false);

            builder.Property(x => x.EndTime)
                .IsRequired(false);

            //  Status (enum)
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>(); // 🔥 guarda como texto en DB

            builder.Property(x => x.CancellationReason)
                .HasMaxLength(500);

            builder.Property(x => x.NonOperationReason)
                .HasMaxLength(200);

            builder.Property(x => x.NonOperationNotes)
                .HasMaxLength(1000);

            builder.Property(x => x.DelayMinutes)
                .IsRequired();

            builder.Property(x => x.IsLate)
                .IsRequired();

            builder.Property(x => x.StartedEarly)
                .IsRequired();

            builder.Property(x => x.EarlyStartReason)
                .HasMaxLength(500);

            builder.Property(x => x.PunctualityStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => x.TripScheduleId);
            builder.HasIndex(x => new { x.TripScheduleId, x.OperationDate })
                .IsUnique()
                .HasFilter("[TripScheduleId] IS NOT NULL AND [OperationDate] IS NOT NULL");

            builder.HasIndex(x => x.RouteAssignmentId)
                .IsUnique()
                .HasFilter("[Status] = 'InProgress'");

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.OperationDate);
            builder.HasIndex(x => x.StartTime);
            builder.HasIndex(x => x.ScheduledDepartureTime);
            builder.HasIndex(x => new { x.RouteAssignmentId, x.Status });
            builder.HasIndex(x => new { x.Status, x.StartTime });
        }
    }
}
