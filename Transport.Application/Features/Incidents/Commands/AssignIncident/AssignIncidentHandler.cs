using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Commands.AssignIncident
{
    public class AssignIncidentHandler : IRequestHandler<AssignIncidentCommand, Unit>
    {
        private readonly IIncidentRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IVisibilityService _visibilityService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;

        public AssignIncidentHandler(IIncidentRepository repository, IUserRepository userRepository, IVisibilityService visibilityService, INotificationService notificationService, IAuditService auditService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _visibilityService = visibilityService;
            _notificationService = notificationService;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(AssignIncidentCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _visibilityService.GetCurrentUserAsync();
            if (!IncidentAccess.IsPrivileged(currentUser))
                throw new DomainException("No tiene permiso para asignar incidentes");

            _ = await _userRepository.GetByIdAsync(request.UserId)
                ?? throw new DomainException("Usuario asignado no encontrado");

            var incident = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Incidente no encontrado");

            incident.AssignTo(request.UserId);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("IncidentAssigned", "Incident", incident.Id.ToString(), null, $"{{\"AssignedToUserId\":\"{request.UserId}\"}}");
            await _notificationService.NotifyUserAsync(request.UserId, "Incidente asignado", $"Se te asignó el incidente {incident.Title}.", Domain.Enums.NotificationType.Incident, Domain.Enums.NotificationPriority.Medium, "Incident", incident.Id.ToString());

            return Unit.Value;
        }
    }
}
