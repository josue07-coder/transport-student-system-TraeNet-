using MediatR;


namespace Transport.Application.Features.Students.Commands.AssignStudentToRoute
{
    public record AssignStudentToRouteCommand(
    Guid StudentId,
    Guid RouteAssignmentId
) : IRequest<Unit>;
}
