using MediatR;
using Transport.Application.Features.TripRouteDeviations.DTOs;

namespace Transport.Application.Features.TripRouteDeviations.Queries.GetTripRouteDeviations
{
    public record GetTripRouteDeviationsQuery(Guid TripId) : IRequest<List<TripRouteDeviationDto>>;
}
