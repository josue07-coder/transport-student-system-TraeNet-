using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentById
{
    public record GetRouteAssignmentByIdQuery(Guid Id) : IRequest<RouteAssignmentDetailDto>;
}
