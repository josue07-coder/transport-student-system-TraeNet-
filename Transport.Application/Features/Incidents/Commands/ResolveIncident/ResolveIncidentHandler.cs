using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Commands.ResolveIncident
{
    public class ResolveIncidentHandler : IRequestHandler<ResolveIncidentCommand, Unit>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public ResolveIncidentHandler(IIncidentRepository repository, IVisibilityService visibilityService, INotificationService notificationService, IUserRepository userRepository, IAuditService auditService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(ResolveIncidentCommand request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            var incident = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Incidente no encontrado");

            if (!IncidentAccess.CanResolve(user, incident))
                throw new DomainException("No tiene permiso para resolver este incidente");

            incident.Resolve(user.Id);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("IncidentResolved", "Incident", incident.Id.ToString(), null, $"{{\"ResolvedByUserId\":\"{user.Id}\"}}");
            await IncidentNotificationHelper.NotifyResolvedAsync(incident, _notificationService, _userRepository);

            return Unit.Value;
        }
    }
}
