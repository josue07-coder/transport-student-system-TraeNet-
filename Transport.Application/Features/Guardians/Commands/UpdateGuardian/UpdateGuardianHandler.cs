using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Guardians.Commands.UpdateGuardian
{
    public class UpdateGuardianHandler : IRequestHandler<UpdateGuardianCommand, Unit>
    {
        private readonly IGuardianRepository _repository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public UpdateGuardianHandler(
            IGuardianRepository repository,
            ISectorRepository sectorRepository,
            IUserRepository userRepository,
            IAuditService auditService)
        {
            _repository = repository;
            _sectorRepository = sectorRepository;
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(UpdateGuardianCommand request, CancellationToken cancellationToken)
        {
            var guardian = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Guardian not found");

            var previousDocumentNumber = guardian.DocumentNumber;
            var oldValues = $"{{\"DocumentNumber\":\"{guardian.DocumentNumber}\",\"FirstName\":\"{guardian.FirstName}\",\"LastName\":\"{guardian.LastName}\"}}";
            var documentChanged = !string.Equals(
                previousDocumentNumber,
                request.DocumentNumber,
                StringComparison.OrdinalIgnoreCase);

            if (await _repository.ExistsByDocumentAsync(request.DocumentNumber, guardian.Id))
                throw new DomainException("Ya existe un acudiente con este documento");

            if (documentChanged && await _userRepository.ExistsByUsernameAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un usuario con este documento");

            if (request.SectorId.HasValue && !await _sectorRepository.ExistsAsync(request.SectorId.Value))
                throw new DomainException("Sector no encontrado");

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

            if (documentChanged)
            {
                var user = await _userRepository.GetByGuardianIdAsync(guardian.Id)
                    ?? throw new DomainException("Usuario vinculado al acudiente no encontrado");

                user.SetUsername(request.DocumentNumber);

                var previousPlaceholderEmail = $"{previousDocumentNumber}@guardian.local";
                if (string.Equals(user.Email, previousPlaceholderEmail, StringComparison.OrdinalIgnoreCase))
                    user.SetEmail($"{request.DocumentNumber}@guardian.local");
            }

            await _repository.SaveChangesAsync();
            var newValues = $"{{\"DocumentNumber\":\"{guardian.DocumentNumber}\",\"FirstName\":\"{guardian.FirstName}\",\"LastName\":\"{guardian.LastName}\"}}";
            await _auditService.LogAsync("Updated", "Guardian", guardian.Id.ToString(), oldValues, newValues);
            return Unit.Value;
        }
    }
}
