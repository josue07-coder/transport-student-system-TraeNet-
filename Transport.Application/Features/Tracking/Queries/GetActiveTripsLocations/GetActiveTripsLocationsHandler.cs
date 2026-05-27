using MediatR;
using Transport.Application.Features.Tracking.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Tracking.Queries.GetActiveTripsLocations
{
    public class GetActiveTripsLocationsHandler : IRequestHandler<GetActiveTripsLocationsQuery, List<CurrentTripLocationDto>>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleLocationRepository _locationRepository;
        private readonly IVisibilityService _visibilityService;

        public GetActiveTripsLocationsHandler(ITripRepository tripRepository, IVehicleLocationRepository locationRepository, IVisibilityService visibilityService)
        {
            _tripRepository = tripRepository;
            _locationRepository = locationRepository;
            _visibilityService = visibilityService;
        }

        public async Task<List<CurrentTripLocationDto>> Handle(GetActiveTripsLocationsQuery request, CancellationToken cancellationToken)
        {
            var trips = await _tripRepository.GetActiveTripsAsync();
            var visibleTrips = await TrackingAccess.FilterTripsAsync(trips, _visibilityService);
            var result = new List<CurrentTripLocationDto>();

            foreach (var trip in visibleTrips)
            {
                var location = await _locationRepository.GetLatestByTripAsync(trip.Id);
                if (location is not null)
                    result.Add(location.ToCurrentDto());
            }

            return result;
        }
    }
}
