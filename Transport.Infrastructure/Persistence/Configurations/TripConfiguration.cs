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

            //  Campos de tiempo
            builder.Property(x => x.StartTime)
                .IsRequired(false);

            builder.Property(x => x.EndTime)
                .IsRequired(false);

            //  Status (enum)
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>(); // 🔥 guarda como texto en DB
        }
    }
}
