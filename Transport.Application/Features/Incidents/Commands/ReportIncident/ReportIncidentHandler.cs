using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Commands.ReportIncident
{
    public class ReportIncidentHandler : IRequestHandler<ReportIncidentCommand, Guid>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IVisibilityService _visibilityService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public ReportIncidentHandler(
            IIncidentRepository incidentRepository,
            ITripRepository tripRepository,
            IRouteAssignmentRepository assignmentRepository,
            IVisibilityService visibilityService,
            INotificationService notificationService,
            IUserRepository userRepository,
            IAuditService auditService)
        {
            _incidentRepository = incidentRepository;
            _tripRepository = tripRepository;
            _assignmentRepository = assignmentRepository;
            _visibilityService = visibilityService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Guid> Handle(ReportIncidentCommand request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();

            if (request.TripId.HasValue)
            {
                var trip = await _tripRepository.GetByIdAsync(request.TripId.Value)
                    ?? throw new DomainException("Viaje no encontrado");
                await _visibilityService.EnsureCanViewTripAsync(trip);
            }

            if (request.RouteAssignmentId.HasValue)
            {
                var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId.Value)
                    ?? throw new DomainException("Asignación de ruta no encontrada");
                await _visibilityService.EnsureCanViewRouteAssignmentAsync(assignment);
            }

            var incident = new Incident(
                request.Title,
                request.Description,
                request.Type,
                request.Severity,
                user.Id,
                request.TripId,
                request.RouteAssignmentId,
                request.VehicleId,
                request.DriverId,
                request.TransportAssistantId);

            await _incidentRepository.AddAsync(incident);
            await _incidentRepository.SaveChangesAsync();

            var incidentWithDetails = await _incidentRepository.GetByIdWithDetailsAsync(incident.Id) ?? incident;
            await _auditService.LogAsync("IncidentReported", "Incident", incident.Id.ToString(), null, $"{{\"Title\":\"{incident.Title}\",\"Severity\":\"{incident.Severity}\"}}");
            await IncidentNotificationHelper.NotifyReportedAsync(incidentWithDetails, _notificationService, _userRepository);

            return incident.Id;
        }
    }
}
