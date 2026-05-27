using MediatR;

namespace Transport.Application.Features.Incidents.Commands.AssignIncident
{
    public class AssignIncidentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
