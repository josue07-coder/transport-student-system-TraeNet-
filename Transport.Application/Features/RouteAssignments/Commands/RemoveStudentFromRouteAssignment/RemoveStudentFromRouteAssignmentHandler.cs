using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.RemoveStudentFromRouteAssignment
{
    public class RemoveStudentFromRouteAssignmentHandler : IRequestHandler<RemoveStudentFromRouteAssignmentCommand, Unit>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public RemoveStudentFromRouteAssignmentHandler(
            IRouteAssignmentRepository assignmentRepository,
            IStudentRepository studentRepository,
            IAuditService auditService,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _assignmentRepository = assignmentRepository;
            _studentRepository = studentRepository;
            _auditService = auditService;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(RemoveStudentFromRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (assignment.Trips.Any(trip => trip.Status == TripStatus.InProgress))
                throw new DomainException("No se puede remover estudiantes mientras existe un viaje en progreso.");

            var student = await _studentRepository.GetByIdIncludingInactiveAsync(request.StudentId)
                ?? throw new DomainException("Estudiante no encontrado");

            assignment.RemoveStudent(request.StudentId);
            await _assignmentRepository.SaveChangesAsync();

            await _auditService.LogAsync("Removed", "RouteAssignment", assignment.Id.ToString(), $"{{\"StudentId\":\"{request.StudentId}\"}}");
            await NotifyGuardianAsync(
                student.GuardianId,
                "Estudiante removido de ruta",
                $"El estudiante {student.FirstName} {student.LastName} fue removido de la ruta {assignment.Route.Name}.",
                assignment.Id);

            return Unit.Value;
        }

        private async Task NotifyGuardianAsync(Guid guardianId, string title, string message, Guid assignmentId)
        {
            var guardianUser = await _userRepository.GetByGuardianIdAsync(guardianId);
            if (guardianUser is null)
                return;

            await _notificationService.NotifyUserAsync(
                guardianUser.Id,
                title,
                message,
                NotificationType.RouteAssignment,
                NotificationPriority.Medium,
                "RouteAssignment",
                assignmentId.ToString());
        }
    }
}
