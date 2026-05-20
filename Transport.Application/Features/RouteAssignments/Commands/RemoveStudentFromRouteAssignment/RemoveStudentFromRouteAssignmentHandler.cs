using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.RemoveStudentFromRouteAssignment
{
    public class RemoveStudentFromRouteAssignmentHandler : IRequestHandler<RemoveStudentFromRouteAssignmentCommand, Unit>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;

        public RemoveStudentFromRouteAssignmentHandler(IRouteAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<Unit> Handle(RemoveStudentFromRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            assignment.RemoveStudent(request.StudentId);
            await _assignmentRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
