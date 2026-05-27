using MediatR;

namespace Transport.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
