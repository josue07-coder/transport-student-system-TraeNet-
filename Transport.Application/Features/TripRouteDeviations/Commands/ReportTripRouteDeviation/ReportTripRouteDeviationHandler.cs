using MediatR;
using System.Data;
using Transport.Application.Features.TripRouteDeviations.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripRouteDeviations.Commands.ReportTripRouteDeviation
{
    public class ReportTripRouteDeviationHandler : IRequestHandler<ReportTripRouteDeviationCommand, TripRouteDeviationDto>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripRouteDeviationRepository _deviationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public ReportTripRouteDeviationHandler(
            ITripRepository tripRepository,
            ITripRouteDeviationRepository deviationRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            INotificationService notificationService,
            ITripOperationAuthorizationService operationAuthorizationService,
            IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _deviationRepository = deviationRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _notificationService = notificationService;
            _operationAuthorizationService = operationAuthorizationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TripRouteDeviationDto> Handle(ReportTripRouteDeviationCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _operationAuthorizationService.EnsureCanReportRouteDeviationAsync(trip);

            var reportedByUserId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            TripRouteDeviation? deviation = null;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                deviation = trip.ReportRouteDeviation(
                    reportedByUserId,
                    request.ReasonType,
                    request.Reason,
                    request.Notes,
                    request.Latitude,
                    request.Longitude,
                    DateTime.UtcNow);

                await _deviationRepository.AddAsync(deviation);
                await _deviationRepository.SaveChangesAsync();
            }, IsolationLevel.ReadCommitted, cancellationToken);

            var reportedDeviation = deviation ?? throw new DomainException("No se pudo registrar el desvío de ruta");

            await _auditService.LogAsync(
                "TripRouteDeviationReported",
                "TripRouteDeviation",
                reportedDeviation.Id.ToString(),
                null,
                $"{{\"TripId\":\"{trip.Id}\",\"UserId\":\"{reportedByUserId}\",\"ReasonType\":\"{request.ReasonType}\",\"Reason\":\"{request.Reason}\"}}");

            await NotifySupervisionAsync(trip.RouteAssignment.Route?.Name, trip.Id);

            var savedDeviation = await _deviationRepository.GetByIdAsync(reportedDeviation.Id)
                ?? reportedDeviation;

            return TripRouteDeviationMappings.ToDto(savedDeviation);
        }

        private async Task NotifySupervisionAsync(string? routeName, Guid tripId)
        {
            var title = "Desvío de ruta reportado";
            var message = string.IsNullOrWhiteSpace(routeName)
                ? "Se reportó un desvío de ruta en un viaje activo."
                : $"Se reportó un desvío en el viaje de la ruta {routeName}.";

            await _notificationService.NotifyRoleAsync(
                "Admin",
                title,
                message,
                NotificationType.Incident,
                NotificationPriority.High,
                "Trip",
                tripId.ToString());

            await _notificationService.NotifyRoleAsync(
                "Supervisor",
                title,
                message,
                NotificationType.Incident,
                NotificationPriority.High,
                "Trip",
                tripId.ToString());
        }
    }
}
