using MediatR;
using Transport.Application.Features.Tracking.DTOs;

namespace Transport.Application.Features.Tracking.Queries.GetTripLocationHistory
{
    public record GetTripLocationHistoryQuery(Guid TripId) : IRequest<List<VehicleLocationResponseDto>>;
}
