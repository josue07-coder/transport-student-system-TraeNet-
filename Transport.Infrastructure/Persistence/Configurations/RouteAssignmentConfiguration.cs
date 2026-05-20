using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class RouteAssignmentConfiguration : IEntityTypeConfiguration<RouteAssignment>
    {
        public void Configure(EntityTypeBuilder<RouteAssignment> builder)
        {
            //  PK
            builder.HasKey(x => x.Id);

            //  Route
            builder.HasOne(x => x.Route)
                .WithMany(r => r.Assignments)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Vehicle
            builder.HasOne(x => x.Vehicle)
                .WithMany()
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Driver
            builder.HasOne(x => x.Driver)
                .WithMany()
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Transport Assistant optional
            builder.HasOne(x => x.TransportAssistant)
                .WithMany()
                .HasForeignKey(x => x.TransportAssistantId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Capacity
            builder.Property(x => x.VehicleCapacity)
                .IsRequired();

            // Students (StudentRouteAssignment)
            builder.HasMany(x => x.Students)
                .WithOne(x => x.RouteAssignment)
                .HasForeignKey(x => x.RouteAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Trips
            builder.HasMany(x => x.Trips)
                .WithOne(x => x.RouteAssignment)
                .HasForeignKey(x => x.RouteAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
