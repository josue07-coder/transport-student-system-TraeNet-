using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class RouteStopConfiguration : IEntityTypeConfiguration<RouteStop>
    {
        public void Configure(EntityTypeBuilder<RouteStop> builder)
        {
            //  Primary Key
            builder.HasKey(x => x.Id);

            //  Relación con Route
            builder.HasOne(x => x.Route)
                .WithMany(r => r.Stops)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Stop
            builder.HasOne(x => x.Stop)
                .WithMany()
                .HasForeignKey(x => x.StopId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Orden
            builder.Property(x => x.StopOrder)
                .IsRequired();

            //  Índice único: evita duplicados en el orden por ruta
            builder.HasIndex(x => new { x.RouteId, x.StopOrder })
                .IsUnique();
        }
    }
}
