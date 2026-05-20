using MediatR;

namespace Transport.Application.Features.TransportAssistants.Commands.DeleteTransportAssistant
{
    public class DeleteTransportAssistantCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
