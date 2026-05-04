using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            //  PK
            builder.HasKey(x => x.Id);

            //  Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            //  School
            builder.Property(x => x.SchoolId)
                .IsRequired();

            builder.HasOne<School>()
                .WithMany()
                .HasForeignKey(x => x.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Grade → Students
            builder.HasMany(x => x.Students)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            //  Índice útil
            builder.HasIndex(x => new { x.Name, x.SchoolId })
                .IsUnique();
        }
    }
}