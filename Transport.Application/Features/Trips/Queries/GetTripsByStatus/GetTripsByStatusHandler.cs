using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Trips.Queries.GetTripsByStatus
{
    public class GetTripsByStatusHandler : IRequestHandler<GetTripsByStatusQuery, List<TripResponseDto>>
    {
        private readonly ITripRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetTripsByStatusHandler(ITripRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<TripResponseDto>> Handle(GetTripsByStatusQuery request, CancellationToken cancellationToken)
        {
            var trips = await _repository.GetByStatusAsync(request.Status);
            trips = await _visibilityService.FilterTripsAsync(trips);
            return trips.Select(TripMappings.ToResponseDto).ToList();
        }
    }
}
