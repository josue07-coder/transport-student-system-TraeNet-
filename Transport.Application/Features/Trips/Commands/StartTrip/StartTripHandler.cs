using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.StartTrip
{
    public class StartTripHandler : IRequestHandler<StartTripCommand, Guid>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public StartTripHandler(
            IRouteAssignmentRepository assignmentRepository,
            ITripRepository tripRepository,
            IAuditService auditService,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _assignmentRepository = assignmentRepository;
            _tripRepository = tripRepository;
            _auditService = auditService;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(StartTripCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (await _tripRepository.HasActiveTripAsync(request.RouteAssignmentId))
                throw new DomainException("Ya hay un viaje activo para esta asignación");

            if (assignment.Route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para iniciar el viaje");

            if (!assignment.Route.Stops.Any())
                throw new DomainException("La ruta debe tener al menos una parada para iniciar el viaje");

            if (!assignment.Driver.IsActive)
                throw new DomainException("El conductor está inactivo");

            if (assignment.Vehicle.Status != VehicleStatus.Active)
                throw new DomainException("El vehiculo está inactivo");

            if (assignment.TransportAssistant is not null && !assignment.TransportAssistant.IsActive)
                throw new DomainException("El asistente de transporte está inactivo");

            if (!assignment.Students.Any())
                throw new DomainException("No se puede iniciar el viaje sin estudiantes");

            var trip = assignment.StartTrip();
            await _tripRepository.AddAsync(trip);
            await _tripRepository.SaveChangesAsync();

            await _auditService.LogAsync("TripStarted", "Trip", trip.Id.ToString(), null, $"{{\"RouteAssignmentId\":\"{trip.RouteAssignmentId}\"}}");
            await NotifyTripStartedAsync(assignment, trip.Id);

            return trip.Id;
        }

        private async Task NotifyTripStartedAsync(Transport.Domain.Entities.RouteAssignment assignment, Guid tripId)
        {
            var routeName = assignment.Route.Name;
            var title = "Viaje iniciado";
            var message = $"El viaje de la ruta {routeName} ha iniciado.";
            var userIds = await GetOperationalAndGuardianUserIdsAsync(assignment);

            await _notificationService.NotifyUsersAsync(
                userIds,
                title,
                message,
                NotificationType.TripStarted,
                NotificationPriority.Medium,
                "Trip",
                tripId.ToString());
        }

        private async Task<List<Guid>> GetOperationalAndGuardianUserIdsAsync(Transport.Domain.Entities.RouteAssignment assignment)
        {
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

            return userIds;
        }
    }
}
