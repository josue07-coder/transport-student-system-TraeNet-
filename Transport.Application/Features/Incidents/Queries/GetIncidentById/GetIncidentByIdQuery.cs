using MediatR;
using Transport.Application.Features.Incidents.DTOs;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentById
{
    public record GetIncidentByIdQuery(Guid Id) : IRequest<IncidentDetailDto>;
}
