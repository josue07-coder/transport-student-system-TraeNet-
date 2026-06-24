using MediatR;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentAbsent;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentBoarded;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentDroppedOff;
using Transport.Application.Features.TripStudentAttendances.Commands.UpdateTripStudentNotes;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Commands
{
    public class TripStudentAttendanceCommandHandler :
        IRequestHandler<MarkStudentBoardedCommand, Unit>,
        IRequestHandler<MarkStudentAbsentCommand, Unit>,
        IRequestHandler<MarkStudentDroppedOffCommand, Unit>,
        IRequestHandler<UpdateTripStudentNotesCommand, Unit>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly IAuditService _auditService;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public TripStudentAttendanceCommandHandler(
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            IAuditService auditService,
            ITripOperationAuthorizationService operationAuthorizationService,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _auditService = auditService;
            _operationAuthorizationService = operationAuthorizationService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<Unit> Handle(MarkStudentBoardedCommand request, CancellationToken cancellationToken)
        {
            var (trip, attendance) = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            var currentUserId = _currentUserService.UserId;
            attendance.MarkBoarded(DateTime.UtcNow, currentUserId);
            await _attendanceRepository.SaveChangesAsync();
            await NotifyGuardianAsync(trip, attendance, true);
            await _auditService.LogAsync("TripStudentMarkedBoarded", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        public async Task<Unit> Handle(MarkStudentAbsentCommand request, CancellationToken cancellationToken)
        {
            var (trip, attendance) = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            var currentUserId = _currentUserService.UserId;
            attendance.MarkAbsent(request.Notes, currentUserId, DateTime.UtcNow);
            await _attendanceRepository.SaveChangesAsync();
            await NotifyGuardianAsync(trip, attendance, false);
            await _auditService.LogAsync("TripStudentMarkedAbsent", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        public async Task<Unit> Handle(MarkStudentDroppedOffCommand request, CancellationToken cancellationToken)
        {
            var (_, attendance) = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            attendance.MarkDroppedOff(DateTime.UtcNow, _currentUserService.UserId);
            await _attendanceRepository.SaveChangesAsync();
            await _auditService.LogAsync("TripStudentMarkedDroppedOff", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateTripStudentNotesCommand request, CancellationToken cancellationToken)
        {
            var (_, attendance) = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            attendance.UpdateNotes(request.Notes);
            await _attendanceRepository.SaveChangesAsync();
            await _auditService.LogAsync("TripStudentNotesUpdated", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        private async Task<(Trip Trip, TripStudentAttendance Attendance)> GetAttendanceForUpdateAsync(Guid tripId, Guid studentId)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(tripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _operationAuthorizationService.EnsureCanManageTripAttendanceAsync(trip);

            if (trip.Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede actualizar la asistencia en un viaje en progreso");

            var attendance = await _attendanceRepository.GetByTripAndStudentAsync(tripId, studentId)
                ?? throw new DomainException("Pasajero no encontrado en este viaje");

            return (trip, attendance);
        }

        private async Task NotifyGuardianAsync(Trip trip, TripStudentAttendance attendance, bool boarded)
        {
            if (!attendance.GuardianIdSnapshot.HasValue)
                return;

            var guardianUser = await _userRepository.GetByGuardianIdAsync(attendance.GuardianIdSnapshot.Value);
            if (guardianUser is null)
                return;

            var routeName = trip.RouteAssignment.Route?.Name ?? "ruta asignada";
            var message = boarded
                ? $"{attendance.StudentNameSnapshot} abordo el transporte escolar de la ruta {routeName} a las {attendance.BoardedAt:HH:mm}."
                : $"{attendance.StudentNameSnapshot} no fue registrado como presente en el viaje de la ruta {routeName}.";

            await _notificationService.NotifyUserAsync(
                guardianUser.Id,
                boarded ? "Estudiante abordo" : "Estudiante ausente",
                message,
                NotificationType.RouteAssignment,
                boarded ? NotificationPriority.Medium : NotificationPriority.High,
                "Trip",
                trip.Id.ToString());

            if (boarded)
                attendance.MarkBoardedNotificationSent(DateTime.UtcNow);
            else
                attendance.MarkAbsenceNotificationSent(DateTime.UtcNow);

            await _attendanceRepository.SaveChangesAsync();
        }
    }
}
