using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Queries.GetTripsByDateRange
{
    public class GetTripsByDateRangeHandler : IRequestHandler<GetTripsByDateRangeQuery, List<TripResponseDto>>
    {
        private readonly ITripRepository _repository;

        public GetTripsByDateRangeHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TripResponseDto>> Handle(GetTripsByDateRangeQuery request, CancellationToken cancellationToken)
        {
            if (request.EndDate.Date < request.StartDate.Date)
                throw new DomainException("La fecha final no puede ser menor que la fecha inicial");

            var trips = await _repository.GetByDateRangeAsync(request.StartDate, request.EndDate);
            return trips.Select(TripMappings.ToResponseDto).ToList();
        }
    }
}
