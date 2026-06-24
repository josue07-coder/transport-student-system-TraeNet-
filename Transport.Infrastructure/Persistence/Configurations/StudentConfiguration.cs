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

                sc.HasIndex(c => c.Value)
                  .IsUnique();
            });
            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(300);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.SchoolId)
                .IsRequired();

            builder.HasOne(s => s.School)
                .WithMany()
                .HasForeignKey(x => x.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Grade)
                 .WithMany(g => g.Students)
                 .HasForeignKey(s => s.GradeId)
                 .OnDelete(DeleteBehavior.Restrict);
            //  GuardianId
            builder.Property(x => x.GuardianId)
                .IsRequired();

            //  Relación con Guardian
            builder.HasOne(s => s.Guardian)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GuardianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SchoolId);
            builder.HasIndex(x => x.GradeId);
            builder.HasIndex(x => x.GuardianId);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
