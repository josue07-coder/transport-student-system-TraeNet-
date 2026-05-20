using MediatR;

namespace Transport.Application.Features.Stops.Commands.DeleteStop
{
    public class DeleteStopCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
