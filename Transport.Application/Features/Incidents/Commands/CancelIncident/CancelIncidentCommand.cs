using MediatR;

namespace Transport.Application.Features.Incidents.Commands.CancelIncident
{
    public class CancelIncidentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
