using MediatR;

namespace Transport.Application.Features.Routes.Commands.CreateRoute
{
    public class CreateRouteCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public Guid SchoolId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
