using MediatR;
using Transport.Application.Features.Routes.DTOs;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Routes.Queries.GetRoutesByStatus
{
    public record GetRoutesByStatusQuery(RouteStatus Status) : IRequest<List<RouteResponseDto>>;
}
