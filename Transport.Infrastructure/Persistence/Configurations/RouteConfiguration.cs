using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            //  Primary Key
            builder.HasKey(x => x.Id);

            // 📌 Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            //  School
            builder.Property(x => x.SchoolId)
                .IsRequired();

            builder.HasOne<School>()
                .WithMany()
                .HasForeignKey(x => x.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Status (enum)
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>();

            // ValueObject: OperatingHours
            builder.OwnsOne(x => x.OperatingHours, th =>
            {
                th.Property(t => t.Start)
                  .HasColumnName("StartTime")
                  .IsRequired();

                th.Property(t => t.End)
                  .HasColumnName("EndTime")
                  .IsRequired();
            });

            //  Route → Assignments
            builder.HasMany(x => x.Assignments)
                .WithOne(x => x.Route)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
