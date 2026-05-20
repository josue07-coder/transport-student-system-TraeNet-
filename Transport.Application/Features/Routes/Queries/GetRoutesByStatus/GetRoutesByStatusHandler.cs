using MediatR;
using Transport.Application.Features.Routes.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Routes.Queries.GetRoutesByStatus
{
    public class GetRoutesByStatusHandler : IRequestHandler<GetRoutesByStatusQuery, List<RouteResponseDto>>
    {
        private readonly IRouteRepository _repository;

        public GetRoutesByStatusHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RouteResponseDto>> Handle(GetRoutesByStatusQuery request, CancellationToken cancellationToken)
        {
            var routes = await _repository.GetByStatusAsync(request.Status);
            return routes.Select(RouteMappings.ToResponseDto).ToList();
        }
    }
}
