using MediatR;

namespace Transport.Application.Features.RouteAssignments.Commands.DeleteRouteAssignment
{
    public class DeleteRouteAssignmentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
