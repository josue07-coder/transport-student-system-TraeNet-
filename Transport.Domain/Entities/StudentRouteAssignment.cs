using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class StudentRouteAssignment: BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid RouteAssignmentId { get; set; }
        public RouteAssignment RouteAssignment { get; set; }

        public Guid StopId { get; set; }
        public Stop Stop { get; set; }
    }
}
