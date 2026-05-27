using MediatR;
using Transport.Application.Features.Tracking.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Tracking.Queries.GetTripLocationHistory
{
    public class GetTripLocationHistoryHandler : IRequestHandler<GetTripLocationHistoryQuery, List<VehicleLocationResponseDto>>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleLocationRepository _locationRepository;
        private readonly IVisibilityService _visibilityService;

        public GetTripLocationHistoryHandler(ITripRepository tripRepository, IVehicleLocationRepository locationRepository, IVisibilityService visibilityService)
        {
            _tripRepository = tripRepository;
            _locationRepository = locationRepository;
            _visibilityService = visibilityService;
        }

        public async Task<List<VehicleLocationResponseDto>> Handle(GetTripLocationHistoryQuery request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await TrackingAccess.EnsureCanViewTripAsync(trip, _visibilityService);

            var locations = await _locationRepository.GetHistoryByTripAsync(request.TripId);
            return locations.Select(location => location.ToResponseDto()).ToList();
        }
    }
}
