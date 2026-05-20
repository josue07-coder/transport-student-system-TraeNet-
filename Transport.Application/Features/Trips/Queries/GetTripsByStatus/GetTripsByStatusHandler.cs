using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Trips.Queries.GetTripsByStatus
{
    public class GetTripsByStatusHandler : IRequestHandler<GetTripsByStatusQuery, List<TripResponseDto>>
    {
        private readonly ITripRepository _repository;

        public GetTripsByStatusHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TripResponseDto>> Handle(GetTripsByStatusQuery request, CancellationToken cancellationToken)
        {
            var trips = await _repository.GetByStatusAsync(request.Status);
            return trips.Select(TripMappings.ToResponseDto).ToList();
        }
    }
}
