using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Routes.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Routes.Queries.GetAllRoutes
{
    public class GetAllRoutesHandler : IRequestHandler<GetAllRoutesQuery, PaginatedResponse<RouteResponseDto>>
    {
        private readonly IRouteRepository _repository;
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IVisibilityService _visibilityService;

        public GetAllRoutesHandler(
            IRouteRepository repository,
            IRouteAssignmentRepository assignmentRepository,
            IVisibilityService visibilityService)
        {
            _repository = repository;
            _assignmentRepository = assignmentRepository;
            _visibilityService = visibilityService;
        }

        public async Task<PaginatedResponse<RouteResponseDto>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
        {
            var routes = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var visibleRoutes = await _visibilityService.FilterRoutesAsync(routes.Items, _assignmentRepository.GetByRouteAsync);
            var items = visibleRoutes.Select(RouteMappings.ToResponseDto).ToList();

            return new PaginatedResponse<RouteResponseDto>(items, visibleRoutes.Count, routes.PageNumber, routes.PageSize);
        }
    }
}
