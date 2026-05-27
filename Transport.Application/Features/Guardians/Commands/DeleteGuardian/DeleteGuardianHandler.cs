using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Guardians.Commands.DeleteGuardian
{
    public class DeleteGuardianHandler : IRequestHandler<DeleteGuardianCommand, Unit>
    {
        private readonly IGuardianRepository _repository;
        private readonly IAuditService _auditService;

        public DeleteGuardianHandler(IGuardianRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeleteGuardianCommand request, CancellationToken cancellationToken)
        {
            var guardian = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Guardian not found");

            if (await _repository.HasActiveStudentsAsync(guardian.Id))
                throw new DomainException("No se puede desactivar el acudiente porque tiene estudiantes activos asociados");

            guardian.Deactivate();
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("Deactivated", "Guardian", guardian.Id.ToString());
            return Unit.Value;
        }
    }
}
