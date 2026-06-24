using MediatR;
using System.Data;
using Transport.Application.Features.TripStudentAttendances.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Commands.AddExceptionalPassenger
{
    public class AddExceptionalPassengerHandler : IRequestHandler<AddExceptionalPassengerCommand, TripStudentAttendanceDto>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public AddExceptionalPassengerHandler(
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            INotificationService notificationService,
            ITripOperationAuthorizationService operationAuthorizationService,
            IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _notificationService = notificationService;
            _operationAuthorizationService = operationAuthorizationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TripStudentAttendanceDto> Handle(AddExceptionalPassengerCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _operationAuthorizationService.EnsureCanManageTripAttendanceAsync(trip);

            if (trip.Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede agregar un pasajero excepcional en un viaje en progreso");

            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DomainException("Estudiante no encontrado");

            if (!student.IsActive)
                throw new DomainException("El estudiante está inactivo");

            if (await _attendanceRepository.ExistsAsync(request.TripId, request.StudentId))
                throw new DomainException("El estudiante ya está registrado como pasajero de este viaje");

            if (trip.RouteAssignment.Students.Any(assignment => assignment.StudentId == request.StudentId))
                throw new DomainException("El estudiante pertenece a la asignación oficial; debe marcarse como abordado usando el flujo normal");

            var currentUserId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            TripStudentAttendance? attendance = null;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (await _attendanceRepository.ExistsAsync(request.TripId, request.StudentId))
                    throw new DomainException("El estudiante ya está registrado como pasajero de este viaje");

                attendance = trip.AddExceptionalPassenger(
                    student.Id,
                    $"{student.FirstName} {student.LastName}",
                    student.StudentCode.Value,
                    student.GuardianId,
                    student.Guardian == null ? null : $"{student.Guardian.FirstName} {student.Guardian.LastName}",
                    request.ExceptionReason,
                    currentUserId,
                    request.BoardedAt ?? DateTime.UtcNow,
                    request.Notes);

                await _attendanceRepository.AddAsync(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }, IsolationLevel.Serializable, cancellationToken);

            await _auditService.LogAsync(
                "ExceptionalPassengerAdded",
                "TripStudentAttendance",
                request.TripId.ToString(),
                null,
                $"{{\"StudentId\":\"{request.StudentId}\",\"RegisteredByUserId\":\"{currentUserId}\",\"ExceptionReason\":\"{request.ExceptionReason}\"}}");

            await NotifySupervisionAsync(request.TripId);

            var savedAttendance = await _attendanceRepository.GetByTripAndStudentAsync(request.TripId, request.StudentId)
                ?? attendance!;

            return TripStudentAttendanceMappings.ToDto(savedAttendance);
        }

        private async Task NotifySupervisionAsync(Guid tripId)
        {
            var title = "Pasajero excepcional registrado";
            var message = $"Se registró un pasajero no asignado en el viaje {tripId}.";

            await _notificationService.NotifyRoleAsync(
                "Admin",
                title,
                message,
                NotificationType.RouteAssignment,
                NotificationPriority.High,
                "Trip",
                tripId.ToString());

            await _notificationService.NotifyRoleAsync(
                "Supervisor",
                title,
                message,
                NotificationType.RouteAssignment,
                NotificationPriority.High,
                "Trip",
                tripId.ToString());
        }
    }
}
