using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            builder.HasKey(incident => incident.Id);

            builder.Property(incident => incident.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(incident => incident.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(incident => incident.Type).IsRequired();
            builder.Property(incident => incident.Severity).IsRequired();
            builder.Property(incident => incident.Status).IsRequired();
            builder.Property(incident => incident.RowVersion).IsRowVersion();

            builder.HasOne(incident => incident.Trip)
                .WithMany()
                .HasForeignKey(incident => incident.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(incident => incident.RouteAssignment)
                .WithMany()
                .HasForeignKey(incident => incident.RouteAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(incident => incident.Vehicle)
                .WithMany()
                .HasForeignKey(incident => incident.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(incident => incident.Driver)
                .WithMany()
                .HasForeignKey(incident => incident.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(incident => incident.TransportAssistant)
                .WithMany()
                .HasForeignKey(incident => incident.TransportAssistantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(incident => incident.ReportedByUser)
                .WithMany()
                .HasForeignKey(incident => incident.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(incident => incident.AssignedToUser)
                .WithMany()
                .HasForeignKey(incident => incident.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(incident => incident.ResolvedByUser)
                .WithMany()
                .HasForeignKey(incident => incident.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(incident => incident.Comments)
                .WithOne(comment => comment.Incident)
                .HasForeignKey(comment => comment.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Incident.Comments))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(incident => incident.Status);
            builder.HasIndex(incident => incident.Severity);
            builder.HasIndex(incident => incident.TripId);
            builder.HasIndex(incident => incident.RouteAssignmentId);
            builder.HasIndex(incident => incident.ReportedByUserId);
            builder.HasIndex(incident => incident.CreatedAt);
            builder.HasIndex(incident => new { incident.Status, incident.CreatedAt });
            builder.HasIndex(incident => new { incident.Severity, incident.CreatedAt });
        }
    }
}
