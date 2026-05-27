using MediatR;

namespace Transport.Application.Features.Incidents.Commands.ResolveIncident
{
    public class ResolveIncidentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
