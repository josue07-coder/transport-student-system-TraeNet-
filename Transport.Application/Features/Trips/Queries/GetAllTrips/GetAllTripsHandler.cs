using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Trips.Queries.GetAllTrips
{
    public class GetAllTripsHandler : IRequestHandler<GetAllTripsQuery, PaginatedResponse<TripResponseDto>>
    {
        private readonly ITripRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetAllTripsHandler(ITripRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<PaginatedResponse<TripResponseDto>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
        {
            var trips = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var visibleTrips = await _visibilityService.FilterTripsAsync(trips.Items);
            var items = visibleTrips.Select(TripMappings.ToResponseDto).ToList();

            return new PaginatedResponse<TripResponseDto>(items, visibleTrips.Count, trips.PageNumber, trips.PageSize);
        }
    }
}
