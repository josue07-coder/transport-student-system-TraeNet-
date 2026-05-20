using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Trips.Queries.GetTripsByRouteAssignment
{
    public class GetTripsByRouteAssignmentHandler : IRequestHandler<GetTripsByRouteAssignmentQuery, List<TripResponseDto>>
    {
        private readonly ITripRepository _repository;

        public GetTripsByRouteAssignmentHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TripResponseDto>> Handle(GetTripsByRouteAssignmentQuery request, CancellationToken cancellationToken)
        {
            var trips = await _repository.GetByRouteAssignmentAsync(request.RouteAssignmentId);
            return trips.Select(TripMappings.ToResponseDto).ToList();
        }
    }
}
