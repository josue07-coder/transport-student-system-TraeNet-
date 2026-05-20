using MediatR;

namespace Transport.Application.Features.RouteAssignments.Commands.RemoveStudentFromRouteAssignment
{
    public class RemoveStudentFromRouteAssignmentCommand : IRequest<Unit>
    {
        public Guid RouteAssignmentId { get; set; }
        public Guid StudentId { get; set; }
    }
}
