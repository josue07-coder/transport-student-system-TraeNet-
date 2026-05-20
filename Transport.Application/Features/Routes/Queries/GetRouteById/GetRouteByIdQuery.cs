using MediatR;
using Transport.Application.Features.Routes.DTOs;

namespace Transport.Application.Features.Routes.Queries.GetRouteById
{
    public record GetRouteByIdQuery(Guid Id) : IRequest<RouteDetailDto>;
}
