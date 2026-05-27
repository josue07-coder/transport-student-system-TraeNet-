using MediatR;
using Transport.Application.Features.Notifications.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Notifications.Queries.GetUnreadNotifications
{
    public class GetUnreadNotificationsHandler : IRequestHandler<GetUnreadNotificationsQuery, List<NotificationResponseDto>>
    {
        private readonly INotificationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetUnreadNotificationsHandler(INotificationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<List<NotificationResponseDto>> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
        {
            Guid? userId = _currentUserService.Role is "Admin" or "Supervisor"
                ? null
                : GetCurrentUserId();

            var notifications = await _repository.GetUnreadAsync(userId);
            return notifications.Select(notification => notification.ToResponseDto()).ToList();
        }

        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId
                ?? throw new DomainException("Usuario autenticado no encontrado");
        }
    }
}
