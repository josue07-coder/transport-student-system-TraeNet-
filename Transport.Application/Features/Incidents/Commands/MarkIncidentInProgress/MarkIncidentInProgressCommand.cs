using MediatR;

namespace Transport.Application.Features.Incidents.Commands.MarkIncidentInProgress
{
    public class MarkIncidentInProgressCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
