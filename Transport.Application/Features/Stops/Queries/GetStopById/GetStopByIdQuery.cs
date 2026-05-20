using MediatR;
using Transport.Application.Features.Stops.DTOs;

namespace Transport.Application.Features.Stops.Queries.GetStopById
{
    public record GetStopByIdQuery(Guid Id) : IRequest<StopDetailDto>;
}
