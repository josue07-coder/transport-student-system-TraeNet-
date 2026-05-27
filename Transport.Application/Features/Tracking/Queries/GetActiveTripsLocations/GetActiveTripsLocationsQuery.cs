using MediatR;
using Transport.Application.Features.Tracking.DTOs;

namespace Transport.Application.Features.Tracking.Queries.GetActiveTripsLocations
{
    public class GetActiveTripsLocationsQuery : IRequest<List<CurrentTripLocationDto>>
    {
    }
}
