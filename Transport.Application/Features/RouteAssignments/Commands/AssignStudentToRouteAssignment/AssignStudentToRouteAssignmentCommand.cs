using MediatR;

namespace Transport.Application.Features.RouteAssignments.Commands.AssignStudentToRouteAssignment
{
    public class AssignStudentToRouteAssignmentCommand : IRequest<Unit>
    {
        public Guid RouteAssignmentId { get; set; }
        public Guid StudentId { get; set; }
    }
}
