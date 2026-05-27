using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentById
{
    public class GetRouteAssignmentByIdHandler : IRequestHandler<GetRouteAssignmentByIdQuery, RouteAssignmentDetailDto>
    {
        private readonly IRouteAssignmentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetRouteAssignmentByIdHandler(IRouteAssignmentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<RouteAssignmentDetailDto> Handle(GetRouteAssignmentByIdQuery request, CancellationToken cancellationToken)
        {
            var assignment = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            await _visibilityService.EnsureCanViewRouteAssignmentAsync(assignment);

            return RouteAssignmentMappings.ToDetailDto(assignment);
        }
    }
}
