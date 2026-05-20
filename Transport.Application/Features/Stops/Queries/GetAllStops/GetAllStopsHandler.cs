using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Stops.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Stops.Queries.GetAllStops
{
    public class GetAllStopsHandler : IRequestHandler<GetAllStopsQuery, PaginatedResponse<StopResponseDto>>
    {
        private readonly IStopRepository _repository;

        public GetAllStopsHandler(IStopRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<StopResponseDto>> Handle(GetAllStopsQuery request, CancellationToken cancellationToken)
        {
            var stops = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = stops.Items.Select(s => new StopResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                SectorId = s.SectorId,
                Street = s.Address.Street,
                City = s.Address.City,
                Latitude = s.Coordinates.Latitude,
                Longitude = s.Coordinates.Longitude
            }).ToList();

            return new PaginatedResponse<StopResponseDto>(items, stops.TotalCount, stops.PageNumber, stops.PageSize);
        }
    }
}
