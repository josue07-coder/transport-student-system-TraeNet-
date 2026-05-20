using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Trips.Queries.GetTripsByStatus
{
    public record GetTripsByStatusQuery(TripStatus Status) : IRequest<List<TripResponseDto>>;
}
