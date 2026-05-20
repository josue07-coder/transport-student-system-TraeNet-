using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Routes.DTOs;

namespace Transport.Application.Features.Routes.Queries.GetAllRoutes
{
    public class GetAllRoutesQuery : PaginationRequest, IRequest<PaginatedResponse<RouteResponseDto>>
    {
    }
}
