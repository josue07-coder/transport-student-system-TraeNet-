using MediatR;

namespace Transport.Application.Features.Routes.Commands.RemoveStopFromRoute
{
    public class RemoveStopFromRouteCommand : IRequest<Unit>
    {
        public Guid RouteId { get; set; }
        public Guid StopId { get; set; }
    }
}
