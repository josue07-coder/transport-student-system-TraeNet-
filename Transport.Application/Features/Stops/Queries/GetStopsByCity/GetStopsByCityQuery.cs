using MediatR;
using Transport.Application.Features.Stops.DTOs;

namespace Transport.Application.Features.Stops.Queries.GetStopsByCity
{
    public record GetStopsByCityQuery(string City) : IRequest<List<StopResponseDto>>;
}
