using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Features.Trips.Queries;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Me.Queries.GetMyTrips
{
    public class GetMyTripsHandler : IRequestHandler<GetMyTripsQuery, List<TripResponseDto>>
    {
        private readonly IVisibilityService _visibilityService;
        private readonly ITripRepository _tripRepository;

        public GetMyTripsHandler(IVisibilityService visibilityService, ITripRepository tripRepository)
        {
            _visibilityService = visibilityService;
            _tripRepository = tripRepository;
        }

        public async Task<List<TripResponseDto>> Handle(GetMyTripsQuery request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();

            var trips = user.DriverId.HasValue
                ? await _tripRepository.GetByDriverAsync(user.DriverId.Value)
                : user.TransportAssistantId.HasValue
                    ? await _tripRepository.GetByTransportAssistantAsync(user.TransportAssistantId.Value)
                    : user.GuardianId.HasValue
                        ? await _tripRepository.GetByGuardianAsync(user.GuardianId.Value)
                        : (await _tripRepository.GetPagedAsync(1, 100)).Items.ToList();

            trips = await _visibilityService.FilterTripsAsync(trips);

            return trips.Select(TripMappings.ToResponseDto).ToList();
        }
    }
}
