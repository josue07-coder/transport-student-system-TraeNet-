using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Transport.Application.Features.Students.Commands.RemoveStudentFromRoute
{
    public record RemoveStudentFromRouteCommand(
    Guid StudentId,
    Guid RouteAssignmentId
    ) : IRequest<Unit>;
}
