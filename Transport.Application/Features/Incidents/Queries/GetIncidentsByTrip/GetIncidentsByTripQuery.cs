using MediatR;
using Transport.Application.Features.Incidents.DTOs;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsByTrip
{
    public record GetIncidentsByTripQuery(Guid TripId) : IRequest<List<IncidentResponseDto>>;
}
