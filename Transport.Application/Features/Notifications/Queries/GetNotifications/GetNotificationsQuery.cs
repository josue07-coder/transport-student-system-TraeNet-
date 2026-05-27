using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Notifications.DTOs;

namespace Transport.Application.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQuery : PaginationRequest, IRequest<PaginatedResponse<NotificationResponseDto>>
    {
    }
}
