using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Queries.GetActiveTripByRouteAssignment
{
    public class GetActiveTripByRouteAssignmentHandler : IRequestHandler<GetActiveTripByRouteAssignmentQuery, TripDetailDto>
    {
        private readonly ITripRepository _repository;

        public GetActiveTripByRouteAssignmentHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<TripDetailDto> Handle(GetActiveTripByRouteAssignmentQuery request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetActiveByRouteAssignmentAsync(request.RouteAssignmentId)
                ?? throw new DomainException("No hay viaje activo para esta asignación");

            return TripMappings.ToDetailDto(trip);
        }
    }
}
