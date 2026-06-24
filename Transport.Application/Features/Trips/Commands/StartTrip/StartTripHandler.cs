using MediatR;
using System.Data;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.StartTrip
{
    public class StartTripHandler : IRequestHandler<StartTripCommand, Guid>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly ISystemSettingService _systemSettingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public StartTripHandler(
            IRouteAssignmentRepository assignmentRepository,
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            IAuditService auditService,
            INotificationService notificationService,
            IUserRepository userRepository,
            ISystemSettingService systemSettingService,
            ICurrentUserService currentUserService,
            ITripOperationAuthorizationService operationAuthorizationService,
            IUnitOfWork unitOfWork)
        {
            _assignmentRepository = assignmentRepository;
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _auditService = auditService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _systemSettingService = systemSettingService;
            _currentUserService = currentUserService;
            _operationAuthorizationService = operationAuthorizationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(StartTripCommand request, CancellationToken cancellationToken)
        {
            var trip = request.TripId.HasValue
                ? await GetScheduledTripAsync(request.TripId.Value, request.RouteAssignmentId)
                : null;

            var assignment = trip?.RouteAssignment ??
                await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (await _tripRepository.HasActiveTripAsync(assignment.Id))
                throw new DomainException("Ya hay un viaje activo para esta asignación");

            await _operationAuthorizationService.EnsureCanStartTripAsync(assignment);
            ValidateAssignmentCanStart(assignment);

            var toleranceMinutes = await _systemSettingService.GetIntAsync("TripStartToleranceMinutes", 5);
            var currentTime = DateTime.UtcNow;
            var canForceEarlyStart = IsAdminOrSupervisor();

            if (request.ForceEarlyStart && !canForceEarlyStart)
                throw new DomainException("Solo Admin o Supervisor pueden autorizar inicio anticipado");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (await _tripRepository.HasActiveTripAsync(assignment.Id))
                    throw new DomainException("Ya hay un viaje activo para esta asignación");

                trip ??= assignment.StartTrip();
                if (request.TripId.HasValue)
                    trip.Start(currentTime, toleranceMinutes, request.ForceEarlyStart && canForceEarlyStart, request.EarlyStartReason);
                else
                    await _tripRepository.AddAsync(trip);

                await SnapshotPassengersAsync(trip, assignment);
                await _tripRepository.SaveChangesAsync();
            }, IsolationLevel.Serializable, cancellationToken);

            var startedTrip = trip ?? throw new DomainException("No se pudo iniciar el viaje");

            await AuditTripStartAsync(startedTrip);
            await _auditService.LogAsync("TripPassengersSnapshotted", "Trip", startedTrip.Id.ToString(), null, $"{{\"StudentsCount\":{assignment.Students.Count}}}");
            await NotifyTripStartedAsync(assignment, startedTrip.Id);

            return startedTrip.Id;
        }

        private bool IsAdminOrSupervisor()
        {
            return string.Equals(_currentUserService.Role, "Admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(_currentUserService.Role, "Supervisor", StringComparison.OrdinalIgnoreCase);
        }

        private async Task AuditTripStartAsync(Trip trip)
        {
            if (trip.StartedEarly)
            {
                await _auditService.LogAsync(
                    "TripStartedEarly",
                    "Trip",
                    trip.Id.ToString(),
                    null,
                    $"{{\"RouteAssignmentId\":\"{trip.RouteAssignmentId}\",\"EarlyStartReason\":\"{trip.EarlyStartReason}\"}}");
                return;
            }

            if (trip.IsLate)
            {
                await _auditService.LogAsync(
                    "TripStartedLate",
                    "Trip",
                    trip.Id.ToString(),
                    null,
                    $"{{\"RouteAssignmentId\":\"{trip.RouteAssignmentId}\",\"DelayMinutes\":{trip.DelayMinutes}}}");
                return;
            }

            await _auditService.LogAsync(
                "TripStarted",
                "Trip",
                trip.Id.ToString(),
                null,
                $"{{\"RouteAssignmentId\":\"{trip.RouteAssignmentId}\"}}");
        }

        private async Task<Trip> GetScheduledTripAsync(Guid tripId, Guid routeAssignmentId)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(tripId)
                ?? throw new DomainException("Viaje no encontrado");

            if (routeAssignmentId != Guid.Empty && trip.RouteAssignmentId != routeAssignmentId)
                throw new DomainException("El viaje no pertenece a la asignación indicada");

            if (trip.Status != TripStatus.Scheduled)
                throw new DomainException("Solo se puede iniciar un viaje programado");

            return trip;
        }

        private static void ValidateAssignmentCanStart(RouteAssignment assignment)
        {
            if (assignment.Route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para iniciar el viaje");

            if (!assignment.Route.Stops.Any())
                throw new DomainException("La ruta debe tener al menos una parada para iniciar el viaje");

            if (!assignment.Driver.IsActive)
                throw new DomainException("El conductor está inactivo");

            if (assignment.Vehicle.Status != VehicleStatus.Active)
                throw new DomainException("El vehículo está inactivo");

            if (assignment.TransportAssistant is not null && !assignment.TransportAssistant.IsActive)
                throw new DomainException("El asistente de transporte está inactivo");

            if (!assignment.Students.Any())
                throw new DomainException("No se puede iniciar el viaje sin estudiantes");
        }

        private async Task SnapshotPassengersAsync(Trip trip, RouteAssignment assignment)
        {
            if (await _attendanceRepository.ExistsForTripAsync(trip.Id))
                throw new DomainException("El viaje ya tiene pasajeros registrados");

            var attendances = assignment.Students
                .Select(studentAssignment => studentAssignment.Student)
                .Where(student => student.IsActive)
                .Select(student => new TripStudentAttendance(
                    trip.Id,
                    student.Id,
                    $"{student.FirstName} {student.LastName}",
                    student.StudentCode.Value,
                    student.GuardianId,
                    student.Guardian == null ? null : $"{student.Guardian.FirstName} {student.Guardian.LastName}"))
                .ToList();

            if (!attendances.Any())
                throw new DomainException("No se puede iniciar el viaje sin estudiantes activos");

            await _attendanceRepository.AddRangeAsync(attendances);
        }

        private async Task NotifyTripStartedAsync(RouteAssignment assignment, Guid tripId)
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

        private async Task<List<Guid>> GetOperationalAndGuardianUserIdsAsync(RouteAssignment assignment)
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
