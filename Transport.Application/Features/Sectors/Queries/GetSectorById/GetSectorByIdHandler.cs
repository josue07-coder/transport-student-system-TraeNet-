using MediatR;
using Transport.Application.Features.Sectors.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Sectors.Queries.GetSectorById
{
    public class GetSectorByIdHandler : IRequestHandler<GetSectorByIdQuery, SectorResponseDto>
    {
        private readonly ISectorRepository _repository;

        public GetSectorByIdHandler(ISectorRepository repository)
        {
            _repository = repository;
        }

        public async Task<SectorResponseDto> Handle(GetSectorByIdQuery request, CancellationToken cancellationToken)
        {
            var sector = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Sector not found");

            return new SectorResponseDto
            {
                Id = sector.Id,
                Name = sector.Name,
                Province = sector.Province,
                City = sector.City
            };
        }
    }
}
