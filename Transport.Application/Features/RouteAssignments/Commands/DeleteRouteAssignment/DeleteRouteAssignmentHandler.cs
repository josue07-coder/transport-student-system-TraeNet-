using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.DeleteRouteAssignment
{
    public class DeleteRouteAssignmentHandler : IRequestHandler<DeleteRouteAssignmentCommand, Unit>
    {
        private readonly IRouteAssignmentRepository _repository;
        private readonly IAuditService _auditService;

        public DeleteRouteAssignmentHandler(IRouteAssignmentRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeleteRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (await _repository.HasTripsAsync(assignment.Id))
                throw new DomainException("No se puede eliminar la asignación porque tiene viajes asociados");

            _repository.Delete(assignment);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("Deleted", "RouteAssignment", assignment.Id.ToString());

            return Unit.Value;
        }
    }
}
