using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Guardians.Commands.UpdateGuardian
{
    public class UpdateGuardianHandler : IRequestHandler<UpdateGuardianCommand, Unit>
    {
        private readonly IGuardianRepository _repository;

        public UpdateGuardianHandler(IGuardianRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateGuardianCommand request, CancellationToken cancellationToken)
        {
            var guardian = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Guardian not found");

            guardian.Update(
                request.DocumentType,
                request.DocumentNumber,
                request.FirstName,
                request.LastName,
                request.Phone,
                Address.Create(request.Street, request.City),
                request.Gender,
                request.SectorId,
                request.PhotoUrl);

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
