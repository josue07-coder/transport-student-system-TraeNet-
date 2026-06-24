using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.EndTrip
{
    public class EndTripHandler : IRequestHandler<EndTripCommand, Unit>
    {
        private readonly ITripRepository _repository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;

        public EndTripHandler(
            ITripRepository repository,
            IAuditService auditService,
            INotificationService notificationService,
            IUserRepository userRepository,
            ITripOperationAuthorizationService operationAuthorizationService)
        {
            _repository = repository;
            _auditService = auditService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _operationAuthorizationService = operationAuthorizationService;
        }

        public async Task<Unit> Handle(EndTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Viaje no encontrado");

            await _operationAuthorizationService.EnsureCanEndTripAsync(trip);
            trip.End();
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("TripEnded", "Trip", trip.Id.ToString(), null, $"{{\"RouteAssignmentId\":\"{trip.RouteAssignmentId}\"}}");
            await NotifyGuardiansAsync(trip, "Viaje finalizado", $"El viaje de la ruta {trip.RouteAssignment.Route.Name} ha finalizado.", NotificationType.TripEnded, NotificationPriority.Medium);

            return Unit.Value;
        }

        private async Task NotifyGuardiansAsync(
            Transport.Domain.Entities.Trip trip,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority)
        {
            var userIds = new List<Guid>();

            foreach (var guardianId in trip.RouteAssignment.Students
                .Select(studentAssignment => studentAssignment.Student.GuardianId)
                .Distinct())
            {
                var guardianUser = await _userRepository.GetByGuardianIdAsync(guardianId);
                if (guardianUser is not null)
                    userIds.Add(guardianUser.Id);
            }

            await _notificationService.NotifyUsersAsync(userIds, title, message, type, priority, "Trip", trip.Id.ToString());
        }
    }
}
