using MediatR;
using Transport.Application.Features.Notifications.DTOs;

namespace Transport.Application.Features.Notifications.Queries.GetUnreadNotifications
{
    public class GetUnreadNotificationsQuery : IRequest<List<NotificationResponseDto>>
    {
    }
}
