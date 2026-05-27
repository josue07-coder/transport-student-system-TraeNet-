using MediatR;
using Transport.Application.Features.Tracking.DTOs;

namespace Transport.Application.Features.Tracking.Queries.GetMyStudentsActiveTripLocations
{
    public class GetMyStudentsActiveTripLocationsQuery : IRequest<List<CurrentTripLocationDto>>
    {
    }
}
