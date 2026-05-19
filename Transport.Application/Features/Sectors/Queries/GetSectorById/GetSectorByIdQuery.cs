using MediatR;
using Transport.Application.Features.Sectors.DTOs;

namespace Transport.Application.Features.Sectors.Queries.GetSectorById
{
    public record GetSectorByIdQuery(Guid Id) : IRequest<SectorResponseDto>;
}
