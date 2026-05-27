using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead
{
    public class MarkAllNotificationsAsReadHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>
    {
        private readonly INotificationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public MarkAllNotificationsAsReadHandler(INotificationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            Guid? userId = _currentUserService.Role is "Admin" or "Supervisor"
                ? null
                : GetCurrentUserId();

            await _repository.MarkAllAsReadAsync(userId);
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }

        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId
                ?? throw new DomainException("Usuario autenticado no encontrado");
        }
    }
}
