using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Guardians.Commands.CreateGuardian
{
    public class CreateGuardianHandler : IRequestHandler<CreateGuardianCommand, Guid>
    {
        private readonly IGuardianRepository _repo;

        public CreateGuardianHandler(IGuardianRepository repo)
        {
            _repo = repo;
        }

        public async Task<Guid> Handle(CreateGuardianCommand request, CancellationToken cancellationToken)
        {
            var guardian = new Guardian(
                documentType: DocumentType.Cedula, //  puedes mejorar luego
                documentNumber: Guid.NewGuid().ToString(),
                firstName: request.FirstName,
                lastName: request.LastName,
                phone: request.Phone,
                address: request.Address,
                gender: Gender.Male, // temporal
                sectorId: request.SectorId
            );

            await _repo.AddAsync(guardian);
            await _repo.SaveChangesAsync();

            return guardian.Id;
        }
    }
}