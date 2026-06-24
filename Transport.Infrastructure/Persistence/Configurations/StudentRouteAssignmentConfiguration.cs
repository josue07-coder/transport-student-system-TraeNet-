using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Configurations
{
    public class StudentRouteAssignmentConfiguration : IEntityTypeConfiguration<StudentRouteAssignment>
    {
        public void Configure(EntityTypeBuilder<StudentRouteAssignment> builder)
        {
            builder.HasKey(x => new { x.StudentId, x.RouteAssignmentId });

            builder.HasOne(x => x.Student)
                .WithMany(s => s.Assignments)
                .HasForeignKey(x => x.StudentId);

            builder.HasOne(x => x.RouteAssignment)
                .WithMany(r => r.Students)
                .HasForeignKey(x => x.RouteAssignmentId);

            builder.HasIndex(x => x.RouteAssignmentId);
        }
    }
}
