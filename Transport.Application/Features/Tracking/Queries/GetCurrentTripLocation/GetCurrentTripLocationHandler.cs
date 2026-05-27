using MediatR;
using Transport.Application.Features.Tracking.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Tracking.Queries.GetCurrentTripLocation
{
    public class GetCurrentTripLocationHandler : IRequestHandler<GetCurrentTripLocationQuery, CurrentTripLocationDto>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleLocationRepository _locationRepository;
        private readonly IVisibilityService _visibilityService;

        public GetCurrentTripLocationHandler(ITripRepository tripRepository, IVehicleLocationRepository locationRepository, IVisibilityService visibilityService)
        {
            _tripRepository = tripRepository;
            _locationRepository = locationRepository;
            _visibilityService = visibilityService;
        }

        public async Task<CurrentTripLocationDto> Handle(GetCurrentTripLocationQuery request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await TrackingAccess.EnsureCanViewTripAsync(trip, _visibilityService);

            var location = await _locationRepository.GetLatestByTripAsync(request.TripId)
                ?? throw new DomainException("Ubicación no encontrada");

            return location.ToCurrentDto();
        }
    }
}
