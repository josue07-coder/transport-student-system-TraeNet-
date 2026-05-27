using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
    {
        private readonly INotificationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public MarkNotificationAsReadHandler(INotificationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Notificación no encontrada");

            if (!IsAdminOrSupervisor() && notification.UserId != GetCurrentUserId())
                throw new DomainException("No tienes permiso para modificar esta notificación");

            notification.MarkAsRead();
            await _repository.SaveChangesAsync();

            return Unit.Value;
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
