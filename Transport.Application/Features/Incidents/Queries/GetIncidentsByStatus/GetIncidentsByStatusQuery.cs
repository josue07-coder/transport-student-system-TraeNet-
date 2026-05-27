using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsByStatus
{
    public record GetIncidentsByStatusQuery(IncidentStatus Status) : IRequest<List<IncidentResponseDto>>;
}
