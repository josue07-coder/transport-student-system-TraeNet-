using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Guardians.Commands.DeleteGuardian
{
    public class DeleteGuardianHandler : IRequestHandler<DeleteGuardianCommand, Unit>
    {
        private readonly IGuardianRepository _repository;

        public DeleteGuardianHandler(IGuardianRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteGuardianCommand request, CancellationToken cancellationToken)
        {
            var guardian = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Guardian not found");

            guardian.Deactivate();
            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
