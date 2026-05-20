using MediatR;
using Transport.Application.Features.Stops.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Stops.Queries.GetStopById
{
    public class GetStopByIdHandler : IRequestHandler<GetStopByIdQuery, StopDetailDto>
    {
        private readonly IStopRepository _repository;

        public GetStopByIdHandler(IStopRepository repository)
        {
            _repository = repository;
        }

        public async Task<StopDetailDto> Handle(GetStopByIdQuery request, CancellationToken cancellationToken)
        {
            var stop = await _repository.GetByIdWithSectorAsync(request.Id)
                ?? throw new DomainException("Parada no encontrada");

            return new StopDetailDto
            {
                Id = stop.Id,
                Name = stop.Name,
                SectorId = stop.SectorId,
                Street = stop.Address.Street,
                City = stop.Address.City,
                Latitude = stop.Coordinates.Latitude,
                Longitude = stop.Coordinates.Longitude
            };
        }
    }
}
