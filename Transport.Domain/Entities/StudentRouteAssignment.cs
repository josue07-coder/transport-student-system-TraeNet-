

namespace Transport.Domain.Entities
{
    public class StudentRouteAssignment
    {
        public Guid StudentId { get; private set; }
        public Guid RouteAssignmentId { get; private set; }

        public StudentRouteAssignment(Guid studentId, Guid routeAssignmentId)
        {
            StudentId = studentId;
            RouteAssignmentId = routeAssignmentId;
        }
    }
}
