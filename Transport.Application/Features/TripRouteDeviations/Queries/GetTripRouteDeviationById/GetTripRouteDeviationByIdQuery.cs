using MediatR;
using Transport.Application.Features.TripRouteDeviations.DTOs;

namespace Transport.Application.Features.TripRouteDeviations.Queries.GetTripRouteDeviationById
{
    public record GetTripRouteDeviationByIdQuery(Guid Id) : IRequest<TripRouteDeviationDto>;
}
