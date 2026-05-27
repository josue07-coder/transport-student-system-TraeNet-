using MediatR;
using Transport.Application.Features.Trips.DTOs;

namespace Transport.Application.Features.Me.Queries.GetMyTrips
{
    public record GetMyTripsQuery : IRequest<List<TripResponseDto>>;
}
