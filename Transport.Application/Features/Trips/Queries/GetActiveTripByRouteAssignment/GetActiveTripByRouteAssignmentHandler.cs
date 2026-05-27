using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Queries.GetActiveTripByRouteAssignment
{
    public class GetActiveTripByRouteAssignmentHandler : IRequestHandler<GetActiveTripByRouteAssignmentQuery, TripDetailDto>
    {
        private readonly ITripRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetActiveTripByRouteAssignmentHandler(ITripRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<TripDetailDto> Handle(GetActiveTripByRouteAssignmentQuery request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetActiveByRouteAssignmentAsync(request.RouteAssignmentId)
                ?? throw new DomainException("No hay viaje activo para esta asignación");

            await _visibilityService.EnsureCanViewTripAsync(trip);

            return TripMappings.ToDetailDto(trip);
        }
    }
}
