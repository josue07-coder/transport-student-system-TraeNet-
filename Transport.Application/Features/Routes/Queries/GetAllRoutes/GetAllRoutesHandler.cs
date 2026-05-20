using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Routes.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Routes.Queries.GetAllRoutes
{
    public class GetAllRoutesHandler : IRequestHandler<GetAllRoutesQuery, PaginatedResponse<RouteResponseDto>>
    {
        private readonly IRouteRepository _repository;

        public GetAllRoutesHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<RouteResponseDto>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
        {
            var routes = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = routes.Items.Select(RouteMappings.ToResponseDto).ToList();

            return new PaginatedResponse<RouteResponseDto>(items, routes.TotalCount, routes.PageNumber, routes.PageSize);
        }
    }
}
