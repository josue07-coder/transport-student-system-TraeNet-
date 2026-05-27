using MediatR;

namespace Transport.Application.Features.Incidents.Commands.CloseIncident
{
    public class CloseIncidentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
