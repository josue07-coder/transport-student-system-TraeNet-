using MediatR;
using System.Data;
using Transport.Application.Features.TripStudentAttendances.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Commands.MarkAttendanceBoarded
{
    public class MarkAttendanceBoardedHandler : IRequestHandler<MarkAttendanceBoardedCommand, TripStudentAttendanceDto>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;
        private readonly ITripOperationAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAttendanceBoardedHandler(
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            IAuditService auditService,
            ITripOperationAuthorizationService authorizationService,
            IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _auditService = auditService;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TripStudentAttendanceDto> Handle(MarkAttendanceBoardedCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _authorizationService.EnsureCanManageTripAttendanceAsync(trip);

            if (trip.Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede registrar asistencia en un viaje en progreso");

            var currentUserId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            var now = DateTime.UtcNow;
            var isOfficialPassenger = trip.RouteAssignment.Students.Any(assignment => assignment.StudentId == request.StudentId);
            TripStudentAttendance? attendance = null;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                attendance = await _attendanceRepository.GetByTripAndStudentAsync(request.TripId, request.StudentId);

                if (isOfficialPassenger)
                {
                    if (attendance is null)
                        throw new DomainException("El estudiante esperado no tiene asistencia creada para este viaje");

                    attendance.MarkBoarded(now, currentUserId);
                    await _attendanceRepository.SaveChangesAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(request.ExceptionReason))
                    throw new DomainException("La razon es obligatoria para registrar un pasajero excepcional");

                if (attendance is not null)
                    throw new DomainException("El estudiante ya esta registrado en la asistencia del viaje");

                var student = await _studentRepository.GetByIdAsync(request.StudentId)
                    ?? throw new DomainException("Estudiante no encontrado");

                attendance = TripStudentAttendance.CreateExceptionalPassenger(
                    trip.Id,
                    student.Id,
                    $"{student.FirstName} {student.LastName}",
                    student.StudentCode.Value,
                    student.GuardianId,
                    student.Guardian == null ? null : $"{student.Guardian.FirstName} {student.Guardian.LastName}",
                    request.ExceptionReason,
                    currentUserId,
                    now,
                    null);

                await _attendanceRepository.AddAsync(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }, IsolationLevel.Serializable, cancellationToken);

            await NotifyBoardedAsync(trip, attendance!, isOfficialPassenger, request.ExceptionReason);
            await _auditService.LogAsync(
                isOfficialPassenger ? "TripStudentMarkedBoarded" : "ExceptionalPassengerAdded",
                "TripStudentAttendance",
                trip.Id.ToString(),
                null,
                $"{{\"StudentId\":\"{request.StudentId}\",\"MarkedByUserId\":\"{currentUserId}\"}}");

            var saved = await _attendanceRepository.GetByTripAndStudentAsync(request.TripId, request.StudentId)
                ?? attendance!;

            return TripStudentAttendanceMappings.ToDto(saved);
        }

        private async Task NotifyBoardedAsync(Trip trip, TripStudentAttendance attendance, bool isOfficialPassenger, string? reason)
        {
            var routeName = trip.RouteAssignment.Route?.Name ?? "ruta asignada";
            var boardedAt = attendance.BoardedAt ?? DateTime.UtcNow;
            var guardianUser = attendance.GuardianIdSnapshot.HasValue
                ? await _userRepository.GetByGuardianIdAsync(attendance.GuardianIdSnapshot.Value)
                : null;

            if (guardianUser is not null)
            {
                var message = isOfficialPassenger
                    ? $"{attendance.StudentNameSnapshot} abordo el transporte escolar de la ruta {routeName} a las {boardedAt:HH:mm}."
                    : $"{attendance.StudentNameSnapshot} abordo una ruta distinta a la asignada. Motivo: {reason}.";

                await _notificationService.NotifyUserAsync(
                    guardianUser.Id,
                    isOfficialPassenger ? "Estudiante abordo" : "Pasajero excepcional",
                    message,
                    NotificationType.RouteAssignment,
                    isOfficialPassenger ? NotificationPriority.Medium : NotificationPriority.High,
                    "Trip",
                    trip.Id.ToString());

                attendance.MarkBoardedNotificationSent(DateTime.UtcNow);
                await _attendanceRepository.SaveChangesAsync();
            }

            if (!isOfficialPassenger)
            {
                await _notificationService.NotifyRoleAsync(
                    "Supervisor",
                    "Pasajero excepcional",
                    $"Se registro un pasajero excepcional en la ruta {routeName}: {attendance.StudentNameSnapshot}.",
                    NotificationType.RouteAssignment,
                    NotificationPriority.High,
                    "Trip",
                    trip.Id.ToString());
            }
        }
    }
}
