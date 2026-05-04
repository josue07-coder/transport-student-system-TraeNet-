using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(300);

            builder.OwnsOne(x => x.StudentCode, sc =>
            {
                sc.Property(c => c.Value)
                  .HasColumnName("StudentCode")
                  .IsRequired();
            });
            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(300);

            builder.Property(x => x.IsActive)
                .IsRequired();
        }
    }
}