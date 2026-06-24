using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Commands.CloseTripAttendance
{
    public class CloseTripAttendanceHandler : IRequestHandler<CloseTripAttendanceCommand>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly ITripOperationAuthorizationService _authorizationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;

        public CloseTripAttendanceHandler(
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            ITripOperationAuthorizationService authorizationService,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            INotificationService notificationService,
            IAuditService auditService)
        {
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _authorizationService = authorizationService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _auditService = auditService;
        }

        public async Task Handle(CloseTripAttendanceCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _authorizationService.EnsureCanManageTripAttendanceAsync(trip);

            if (trip.Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede cerrar asistencia en un viaje en progreso");

            var now = DateTime.UtcNow;
            var currentUserId = _currentUserService.UserId;
            var attendances = await _attendanceRepository.GetByTripAsync(request.TripId);
            var expectedNotMarked = attendances
                .Where(attendance => attendance.IsExpectedPassenger && attendance.Status == TripAttendanceStatus.Expected)
                .ToList();

            foreach (var attendance in expectedNotMarked)
            {
                attendance.MarkAbsent("Cierre automatico de asistencia", currentUserId, now);
            }

            await _attendanceRepository.SaveChangesAsync();

            foreach (var attendance in expectedNotMarked)
            {
                await NotifyAbsentAsync(trip.Id, trip.RouteAssignment.Route?.Name, attendance);
            }

            await _auditService.LogAsync(
                "TripAttendanceClosed",
                "TripStudentAttendance",
                request.TripId.ToString(),
                null,
                $"{{\"AbsentMarked\":{expectedNotMarked.Count}}}");
        }

        private async Task NotifyAbsentAsync(Guid tripId, string? routeName, Transport.Domain.Entities.TripStudentAttendance attendance)
        {
            if (!attendance.GuardianIdSnapshot.HasValue)
                return;

            var guardianUser = await _userRepository.GetByGuardianIdAsync(attendance.GuardianIdSnapshot.Value);
            if (guardianUser is null)
                return;

            await _notificationService.NotifyUserAsync(
                guardianUser.Id,
                "Estudiante ausente",
                $"{attendance.StudentNameSnapshot} no fue registrado como presente en el viaje de la ruta {routeName ?? "asignada"}.",
                NotificationType.RouteAssignment,
                NotificationPriority.High,
                "Trip",
                tripId.ToString());

            attendance.MarkAbsenceNotificationSent(DateTime.UtcNow);
            await _attendanceRepository.SaveChangesAsync();
        }
    }
}
