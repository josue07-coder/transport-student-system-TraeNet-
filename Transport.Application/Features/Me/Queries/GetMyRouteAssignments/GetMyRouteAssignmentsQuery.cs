using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;

namespace Transport.Application.Features.Me.Queries.GetMyRouteAssignments
{
    public record GetMyRouteAssignmentsQuery : IRequest<List<RouteAssignmentResponseDto>>;
}
