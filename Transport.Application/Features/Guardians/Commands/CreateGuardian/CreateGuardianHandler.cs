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
                 request.DocumentType,
                 request.DocumentNumber,
                 request.FirstName,
                 request.LastName,
                 request.Phone,
                 Address.Create(request.Street, request.City),
                 request.Gender,
                 request.SectorId
            );

            await _repo.AddAsync(guardian);
            await _repo.SaveChangesAsync();

            return guardian.Id;
        }
    }
}