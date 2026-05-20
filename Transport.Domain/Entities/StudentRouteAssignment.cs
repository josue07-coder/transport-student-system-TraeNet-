

namespace Transport.Domain.Entities
{
    public class StudentRouteAssignment
    {
        public Guid StudentId { get; private set; }
        public Guid RouteAssignmentId { get; private set; }

        public Student Student { get; private set; } = null!;
        public RouteAssignment RouteAssignment { get; private set; } = null!;

        public StudentRouteAssignment(Guid studentId, Guid routeAssignmentId)
        {
            StudentId = studentId;
            RouteAssignmentId = routeAssignmentId;
        }
    }
}
