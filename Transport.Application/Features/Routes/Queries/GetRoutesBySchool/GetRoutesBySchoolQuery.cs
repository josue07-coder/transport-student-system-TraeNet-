using MediatR;
using Transport.Application.Features.Routes.DTOs;

namespace Transport.Application.Features.Routes.Queries.GetRoutesBySchool
{
    public record GetRoutesBySchoolQuery(Guid SchoolId) : IRequest<List<RouteResponseDto>>;
}
