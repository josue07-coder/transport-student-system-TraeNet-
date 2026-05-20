using MediatR;
using Transport.Application.Features.Stops.DTOs;

namespace Transport.Application.Features.Stops.Queries.GetStopsBySector
{
    public record GetStopsBySectorQuery(Guid SectorId) : IRequest<List<StopResponseDto>>;
}
