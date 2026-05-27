using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Trips.Queries.GetTripsByRouteAssignment
{
    public class GetTripsByRouteAssignmentHandler : IRequestHandler<GetTripsByRouteAssignmentQuery, List<TripResponseDto>>
    {
        private readonly ITripRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetTripsByRouteAssignmentHandler(ITripRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<TripResponseDto>> Handle(GetTripsByRouteAssignmentQuery request, CancellationToken cancellationToken)
        {
            var trips = await _repository.GetByRouteAssignmentAsync(request.RouteAssignmentId);
            trips = await _visibilityService.FilterTripsAsync(trips);
            return trips.Select(TripMappings.ToResponseDto).ToList();
        }
    }
}
