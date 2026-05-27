using MediatR;
using Transport.Application.Features.Routes.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Routes.Queries.GetRouteById
{
    public class GetRouteByIdHandler : IRequestHandler<GetRouteByIdQuery, RouteDetailDto>
    {
        private readonly IRouteRepository _repository;
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IVisibilityService _visibilityService;

        public GetRouteByIdHandler(
            IRouteRepository repository,
            IRouteAssignmentRepository assignmentRepository,
            IVisibilityService visibilityService)
        {
            _repository = repository;
            _assignmentRepository = assignmentRepository;
            _visibilityService = visibilityService;
        }

        public async Task<RouteDetailDto> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdWithStopsAsync(request.Id)
                ?? throw new DomainException("Ruta no encontrada");

            var visibleRoutes = await _visibilityService.FilterRoutesAsync(
                new[] { route },
                _assignmentRepository.GetByRouteAsync);

            if (!visibleRoutes.Any())
                throw new DomainException("No tiene permiso para consultar esta ruta");

            return RouteMappings.ToDetailDto(route);
        }
    }
}
