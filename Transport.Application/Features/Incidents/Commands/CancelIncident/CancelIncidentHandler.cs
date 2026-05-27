using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Commands.CancelIncident
{
    public class CancelIncidentHandler : IRequestHandler<CancelIncidentCommand, Unit>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;
        private readonly IAuditService _auditService;

        public CancelIncidentHandler(IIncidentRepository repository, IVisibilityService visibilityService, IAuditService auditService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(CancelIncidentCommand request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (!IncidentAccess.IsPrivileged(user))
                throw new DomainException("No tiene permiso para cancelar incidentes");

            var incident = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Incidente no encontrado");

            incident.Cancel();
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("IncidentCancelled", "Incident", incident.Id.ToString());

            return Unit.Value;
        }
    }
}
