using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Commands.CloseIncident
{
    public class CloseIncidentHandler : IRequestHandler<CloseIncidentCommand, Unit>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;
        private readonly IAuditService _auditService;

        public CloseIncidentHandler(IIncidentRepository repository, IVisibilityService visibilityService, IAuditService auditService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(CloseIncidentCommand request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (!IncidentAccess.IsPrivileged(user))
                throw new DomainException("No tiene permiso para cerrar incidentes");

            var incident = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Incidente no encontrado");

            incident.Close();
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("IncidentClosed", "Incident", incident.Id.ToString());

            return Unit.Value;
        }
    }
}
