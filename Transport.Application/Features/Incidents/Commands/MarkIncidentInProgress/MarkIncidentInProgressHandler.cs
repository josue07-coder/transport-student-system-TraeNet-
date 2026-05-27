using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Commands.MarkIncidentInProgress
{
    public class MarkIncidentInProgressHandler : IRequestHandler<MarkIncidentInProgressCommand, Unit>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;
        private readonly IAuditService _auditService;

        public MarkIncidentInProgressHandler(IIncidentRepository repository, IVisibilityService visibilityService, IAuditService auditService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(MarkIncidentInProgressCommand request, CancellationToken cancellationToken)
        {
            var incident = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Incidente no encontrado");

            await IncidentAccess.EnsureCanViewAsync(incident, _visibilityService);
            incident.MarkInProgress();
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("IncidentInProgress", "Incident", incident.Id.ToString());

            return Unit.Value;
        }
    }
}
