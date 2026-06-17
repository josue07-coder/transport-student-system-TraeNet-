using MediatR;
using System.Text.Json;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.CancelTrip
{
    public class CancelTripHandler : IRequestHandler<CancelTripCommand, Unit>
    {
        private readonly ITripRepository _repository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public CancelTripHandler(
            ITripRepository repository,
            IAuditService auditService,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _repository = repository;
            _auditService = auditService;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Viaje no encontrado");

            trip.Cancel(request.Reason);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync(
                "TripCancelled",
                "Trip",
                trip.Id.ToString(),
                null,
                JsonSerializer.Serialize(new
                {
                    trip.RouteAssignmentId,
                    Reason = trip.CancellationReason
                }));
            await NotifyTripCancelledAsync(trip);

            return Unit.Value;
        }

        private async Task NotifyTripCancelledAsync(Transport.Domain.Entities.Trip trip)
        {
            var assignment = trip.RouteAssignment;
            var userIds = new List<Guid>();

            var driverUser = await _userRepository.GetByDriverIdAsync(assignment.DriverId);
            if (driverUser is not null)
                userIds.Add(driverUser.Id);

            if (assignment.TransportAssistantId.HasValue)
            {
                var assistantUser = await _userRepository.GetByTransportAssistantIdAsync(assignment.TransportAssistantId.Value);
                if (assistantUser is not null)
                    userIds.Add(assistantUser.Id);
            }

            foreach (var guardianId in assignment.Students
                .Select(studentAssignment => studentAssignment.Student.GuardianId)
                .Distinct())
            {
                var guardianUser = await _userRepository.GetByGuardianIdAsync(guardianId);
                if (guardianUser is not null)
                    userIds.Add(guardianUser.Id);
            }

            var title = "Viaje cancelado";
            var message = $"El viaje de la ruta {assignment.Route.Name} fue cancelado. Motivo: {trip.CancellationReason}.";

            await _notificationService.NotifyUsersAsync(
                userIds,
                title,
                message,
                NotificationType.TripCancelled,
                NotificationPriority.High,
                "Trip",
                trip.Id.ToString());

            await _notificationService.NotifyRoleAsync(
                "Supervisor",
                title,
                message,
                NotificationType.TripCancelled,
                NotificationPriority.High,
                "Trip",
                trip.Id.ToString());
        }
    }
}
