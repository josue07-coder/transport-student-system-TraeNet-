using MediatR;

namespace Transport.Application.Features.Drivers.Commands.DeleteDriver
{
    public class DeleteDriverCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
