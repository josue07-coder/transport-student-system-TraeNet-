using MediatR;

namespace Transport.Application.Features.Routes.Commands.UpdateRouteStopOrder
{
    public class UpdateRouteStopOrderCommand : IRequest<Unit>
    {
        public Guid RouteId { get; set; }
        public Guid StopId { get; set; }
        public int StopOrder { get; set; }
    }
}
