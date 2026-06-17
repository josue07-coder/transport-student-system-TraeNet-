using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class NonSchoolDayConfiguration : IEntityTypeConfiguration<NonSchoolDay>
    {
        public void Configure(EntityTypeBuilder<NonSchoolDay> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.ReasonType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.School)
                .WithMany()
                .HasForeignKey(x => x.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => new { x.Date, x.IsActive });
            builder.HasIndex(x => new { x.Date, x.SchoolId })
                .IsUnique()
                .HasFilter("[SchoolId] IS NOT NULL AND [IsActive] = 1");
            builder.HasIndex(x => x.Date)
                .IsUnique()
                .HasDatabaseName("IX_NonSchoolDays_Date_Global_Active")
                .HasFilter("[SchoolId] IS NULL AND [IsActive] = 1");
        }
    }
}
