using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Trips.Queries.GetAllTrips
{
    public class GetAllTripsHandler : IRequestHandler<GetAllTripsQuery, PaginatedResponse<TripResponseDto>>
    {
        private readonly ITripRepository _repository;

        public GetAllTripsHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<TripResponseDto>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
        {
            var trips = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = trips.Items.Select(TripMappings.ToResponseDto).ToList();

            return new PaginatedResponse<TripResponseDto>(items, trips.TotalCount, trips.PageNumber, trips.PageSize);
        }
    }
}
