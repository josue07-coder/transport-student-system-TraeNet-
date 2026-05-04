using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            //  Primary Key
            builder.HasKey(x => x.Id);

            // 🚗 PlateNumber
            builder.Property(x => x.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            //  Índice único (placa no se repite)
            builder.HasIndex(x => x.PlateNumber)
                .IsUnique();

            // Capacity
            builder.Property(x => x.Capacity)
                .IsRequired();

            //  Status (enum)
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}