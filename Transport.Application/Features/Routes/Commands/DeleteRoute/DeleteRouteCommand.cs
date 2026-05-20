using MediatR;

namespace Transport.Application.Features.Routes.Commands.DeleteRoute
{
    public class DeleteRouteCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
