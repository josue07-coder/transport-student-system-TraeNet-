using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Notifications.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, PaginatedResponse<NotificationResponseDto>>
    {
        private readonly INotificationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetNotificationsHandler(INotificationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResponse<NotificationResponseDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = IsAdminOrSupervisor()
                ? await _repository.GetPagedAsync(request.PageNumber, request.PageSize)
                : await _repository.GetByUserAsync(GetCurrentUserId(), request.PageNumber, request.PageSize);

            var items = notifications.Items.Select(notification => notification.ToResponseDto()).ToList();
            return new PaginatedResponse<NotificationResponseDto>(
                items,
                notifications.TotalCount,
                notifications.PageNumber,
                notifications.PageSize);
        }

        private bool IsAdminOrSupervisor()
        {
            return _currentUserService.Role is "Admin" or "Supervisor";
        }

        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId
                ?? throw new DomainException("Usuario autenticado no encontrado");
        }
    }
}
