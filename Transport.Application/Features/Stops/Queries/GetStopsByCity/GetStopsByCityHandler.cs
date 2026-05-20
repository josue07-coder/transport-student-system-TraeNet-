using MediatR;
using Transport.Application.Features.Stops.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Stops.Queries.GetStopsByCity
{
    public class GetStopsByCityHandler : IRequestHandler<GetStopsByCityQuery, List<StopResponseDto>>
    {
        private readonly IStopRepository _repository;

        public GetStopsByCityHandler(IStopRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StopResponseDto>> Handle(GetStopsByCityQuery request, CancellationToken cancellationToken)
        {
            var stops = await _repository.GetByCityAsync(request.City);

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
