using MediatR;
using Transport.Application.Features.Tracking.DTOs;

namespace Transport.Application.Features.Tracking.Queries.GetCurrentTripLocation
{
    public record GetCurrentTripLocationQuery(Guid TripId) : IRequest<CurrentTripLocationDto>;
}
