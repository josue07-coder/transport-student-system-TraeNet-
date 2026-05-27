using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Students.Commands.DeleteStudent
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, Unit>
    {
        private readonly IStudentRepository _repo;
        private readonly IAuditService _auditService;

        public DeleteStudentHandler(IStudentRepository repo, IAuditService auditService)
        {
            _repo = repo;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _repo.GetByIdAsync(request.Id);

            if (student == null)
                throw new DomainException("Student not found");

            if (await _repo.HasActiveRouteAssignmentAsync(student.Id))
                throw new DomainException("No se puede desactivar el estudiante porque tiene asignaciones de ruta activas");

            if (await _repo.HasInProgressTripAsync(student.Id))
                throw new DomainException("No se puede desactivar el estudiante porque tiene un viaje en progreso");

            student.Deactivate();

            await _repo.SaveChangesAsync();

            await _auditService.LogAsync("Deactivated", "Student", student.Id.ToString());

            return Unit.Value;
        }
    }
}
