using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentById
{
    public class GetRouteAssignmentByIdHandler : IRequestHandler<GetRouteAssignmentByIdQuery, RouteAssignmentDetailDto>
    {
        private readonly IRouteAssignmentRepository _repository;

        public GetRouteAssignmentByIdHandler(IRouteAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<RouteAssignmentDetailDto> Handle(GetRouteAssignmentByIdQuery request, CancellationToken cancellationToken)
        {
            var assignment = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            return RouteAssignmentMappings.ToDetailDto(assignment);
        }
    }
}
