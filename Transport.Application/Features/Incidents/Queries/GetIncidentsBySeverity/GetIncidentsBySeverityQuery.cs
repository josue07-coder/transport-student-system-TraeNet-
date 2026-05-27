using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsBySeverity
{
    public record GetIncidentsBySeverityQuery(IncidentSeverity Severity) : IRequest<List<IncidentResponseDto>>;
}
