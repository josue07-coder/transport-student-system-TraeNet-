using MediatR;
using System.Text.Json;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.MarkTripNotOperating
{
    public class MarkTripNotOperatingHandler : IRequestHandler<MarkTripNotOperatingCommand, Unit>
    {
        private readonly ITripRepository _repository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;

        public MarkTripNotOperatingHandler(
            ITripRepository repository,
            IAuditService auditService,
            INotificationService notificationService)
        {
            _repository = repository;
            _auditService = auditService;
            _notificationService = notificationService;
        }

        public async Task<Unit> Handle(MarkTripNotOperatingCommand request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Viaje no encontrado");

            trip.MarkNotOperating(request.Reason, request.Notes);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync(
                "TripMarkedNotOperating",
                "Trip",
                trip.Id.ToString(),
                null,
                JsonSerializer.Serialize(new
                {
                    trip.RouteAssignmentId,
                    Reason = trip.NonOperationReason,
                    Notes = trip.NonOperationNotes
                }));

            await _notificationService.NotifyRoleAsync(
                "Supervisor",
                "Viaje marcado sin operación",
                $"El viaje de la ruta {trip.RouteAssignment.Route.Name} no operará. Motivo: {trip.NonOperationReason}.",
                NotificationType.System,
                NotificationPriority.Medium,
                "Trip",
                trip.Id.ToString());

            return Unit.Value;
        }
    }
}
