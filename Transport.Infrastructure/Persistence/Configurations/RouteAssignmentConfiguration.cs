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
            builder.HasOne<Route>()
                .WithMany(r => r.Assignments)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Vehicle
            builder.HasOne<Vehicle>()
                .WithMany()
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Driver
            builder.HasOne<Driver>()
                .WithMany()
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Capacity
            builder.Property(x => x.VehicleCapacity)
                .IsRequired();

            // Students (StudentRouteAssignment)
            builder.HasMany(x => x.Students)
                .WithOne()
                .HasForeignKey(x => x.RouteAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Trips
            builder.HasMany(x => x.Trips)
                .WithOne()
                .HasForeignKey(x => x.RouteAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}