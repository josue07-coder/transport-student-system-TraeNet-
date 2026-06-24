using MediatR;
using System.Data;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.AssignStudentToRouteAssignment
{
    public class AssignStudentToRouteAssignmentHandler : IRequestHandler<AssignStudentToRouteAssignmentCommand, Unit>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignStudentToRouteAssignmentHandler(
            IRouteAssignmentRepository assignmentRepository,
            IStudentRepository studentRepository,
            IAuditService auditService,
            INotificationService notificationService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _assignmentRepository = assignmentRepository;
            _studentRepository = studentRepository;
            _auditService = auditService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AssignStudentToRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            RouteAssignment? assignment = null;
            Student? student = null;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                    ?? throw new DomainException("Asignación de ruta no encontrada");

                student = await _studentRepository.GetByIdIncludingInactiveAsync(request.StudentId)
                    ?? throw new DomainException("Estudiante no encontrado");

                if (!student.IsActive)
                    throw new DomainException("El estudiante está inactivo");

                if (assignment.Route.Status != RouteStatus.Active)
                    throw new DomainException("La ruta debe estar activa para asignar estudiantes");

                if (student.SchoolId != assignment.Route.SchoolId)
                    throw new DomainException("El estudiante pertenece a una escuela distinta a la ruta");

                if (assignment.Students.Any(studentAssignment => studentAssignment.StudentId == request.StudentId))
                    throw new DomainException("El estudiante ya está asignado a esta asignación");

                if (assignment.Students.Count >= assignment.VehicleCapacity)
                    throw new DomainException("Capacidad del vehículo excedida");

                if (await _assignmentRepository.HasStudentScheduleConflictAsync(request.StudentId, assignment.RouteId, assignment.Id))
                    throw new DomainException("El estudiante ya tiene una asignación activa con horario cruzado");

                assignment.AssignStudent(request.StudentId);
                await _assignmentRepository.SaveChangesAsync();
            }, IsolationLevel.Serializable, cancellationToken);

            await _auditService.LogAsync("Assigned", "RouteAssignment", assignment!.Id.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            await NotifyGuardianAsync(
                student!.GuardianId,
                "Estudiante asignado a ruta",
                $"El estudiante {student.FirstName} {student.LastName} fue asignado a la ruta {assignment.Route.Name}.",
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
