using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.AssignStudentToRouteAssignment
{
    public class AssignStudentToRouteAssignmentHandler : IRequestHandler<AssignStudentToRouteAssignmentCommand, Unit>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IStudentRepository _studentRepository;

        public AssignStudentToRouteAssignmentHandler(
            IRouteAssignmentRepository assignmentRepository,
            IStudentRepository studentRepository)
        {
            _assignmentRepository = assignmentRepository;
            _studentRepository = studentRepository;
        }

        public async Task<Unit> Handle(AssignStudentToRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (!await _studentRepository.ExistsAsync(request.StudentId))
                throw new DomainException("Estudiante no encontrado");

            assignment.AssignStudent(request.StudentId);
            await _assignmentRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
