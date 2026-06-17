using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class TripStudentAttendanceConfiguration : IEntityTypeConfiguration<TripStudentAttendance>
    {
        public void Configure(EntityTypeBuilder<TripStudentAttendance> builder)
        {
            builder.HasKey(x => new { x.TripId, x.StudentId });

            builder.Property(x => x.StudentNameSnapshot)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.StudentCodeSnapshot)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.GuardianNameSnapshot)
                .HasMaxLength(250);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Trip)
                .WithMany(x => x.StudentAttendances)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TripId);
            builder.HasIndex(x => x.StudentId);
            builder.HasIndex(x => x.GuardianIdSnapshot);
        }
    }
}
