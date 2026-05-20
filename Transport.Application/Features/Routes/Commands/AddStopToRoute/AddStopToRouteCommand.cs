using MediatR;

namespace Transport.Application.Features.Routes.Commands.AddStopToRoute
{
    public class AddStopToRouteCommand : IRequest<Unit>
    {
        public Guid RouteId { get; set; }
        public Guid StopId { get; set; }
        public int StopOrder { get; set; }
    }
}
