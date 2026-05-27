using MediatR;
using Transport.Application.Features.Routes.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Routes.Queries.GetRoutesByStatus
{
    public class GetRoutesByStatusHandler : IRequestHandler<GetRoutesByStatusQuery, List<RouteResponseDto>>
    {
        private readonly IRouteRepository _repository;
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IVisibilityService _visibilityService;

        public GetRoutesByStatusHandler(
            IRouteRepository repository,
            IRouteAssignmentRepository assignmentRepository,
            IVisibilityService visibilityService)
        {
            _repository = repository;
            _assignmentRepository = assignmentRepository;
            _visibilityService = visibilityService;
        }

        public async Task<List<RouteResponseDto>> Handle(GetRoutesByStatusQuery request, CancellationToken cancellationToken)
        {
            var routes = await _repository.GetByStatusAsync(request.Status);
            routes = await _visibilityService.FilterRoutesAsync(routes, _assignmentRepository.GetByRouteAsync);
            return routes.Select(RouteMappings.ToResponseDto).ToList();
        }
    }
}
