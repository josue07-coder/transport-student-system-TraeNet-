using MediatR;
using Transport.Application.Features.Tracking.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Tracking.Queries.GetMyStudentsActiveTripLocations
{
    public class GetMyStudentsActiveTripLocationsHandler : IRequestHandler<GetMyStudentsActiveTripLocationsQuery, List<CurrentTripLocationDto>>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleLocationRepository _locationRepository;
        private readonly IVisibilityService _visibilityService;

        public GetMyStudentsActiveTripLocationsHandler(ITripRepository tripRepository, IVehicleLocationRepository locationRepository, IVisibilityService visibilityService)
        {
            _tripRepository = tripRepository;
            _locationRepository = locationRepository;
            _visibilityService = visibilityService;
        }

        public async Task<List<CurrentTripLocationDto>> Handle(GetMyStudentsActiveTripLocationsQuery request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (!user.GuardianId.HasValue)
                throw new DomainException("El usuario no tiene tutor asociado");

            var trips = await _tripRepository.GetByGuardianAsync(user.GuardianId.Value);
            var activeTrips = trips.Where(trip => trip.IsActive).ToList();
            var result = new List<CurrentTripLocationDto>();

            foreach (var trip in activeTrips)
            {
                var location = await _locationRepository.GetLatestByTripAsync(trip.Id);
                if (location is not null)
                    result.Add(location.ToCurrentDto());
            }

            return result;
        }
    }
}
