using MediatR;
using Transport.Application.Features.Stops.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Stops.Queries.GetStopsBySector
{
    public class GetStopsBySectorHandler : IRequestHandler<GetStopsBySectorQuery, List<StopResponseDto>>
    {
        private readonly IStopRepository _repository;

        public GetStopsBySectorHandler(IStopRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StopResponseDto>> Handle(GetStopsBySectorQuery request, CancellationToken cancellationToken)
        {
            var stops = await _repository.GetBySectorAsync(request.SectorId);

            return stops.Select(s => new StopResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                SectorId = s.SectorId,
                Street = s.Address.Street,
                City = s.Address.City,
                Latitude = s.Coordinates.Latitude,
                Longitude = s.Coordinates.Longitude
            }).ToList();
        }
    }
}
