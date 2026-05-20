using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Routes.Commands.UpdateRoute
{
    public class UpdateRouteCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SchoolId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public RouteStatus Status { get; set; }
    }
}
