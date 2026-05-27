using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Guardians.Commands.CreateGuardian
{
    public class CreateGuardianHandler : IRequestHandler<CreateGuardianCommand, Guid>
    {
        private readonly IGuardianRepository _guardianRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ISectorRepository _sectorRepository;
        private readonly IAuditService _auditService;

        public CreateGuardianHandler(
            IGuardianRepository guardianRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasherService passwordHasher,
            ISectorRepository sectorRepository,
            IAuditService auditService)
        {
            _guardianRepository = guardianRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _sectorRepository = sectorRepository;
            _auditService = auditService;
        }

        public async Task<Guid> Handle(CreateGuardianCommand request, CancellationToken cancellationToken)
        {
            if (await _guardianRepository.ExistsByDocumentAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un acudiente con este documento");

            if (await _userRepository.ExistsByUsernameAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un usuario con este documento");

            if (request.SectorId.HasValue && !await _sectorRepository.ExistsAsync(request.SectorId.Value))
                throw new DomainException("Sector no encontrado");

            var role = await _roleRepository.GetByNameAsync("Guardian")
                ?? throw new DomainException("Rol Guardian no encontrado");

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

            var user = new User(
                request.DocumentNumber,
                $"{request.FirstName} {request.LastName}",
                $"{request.DocumentNumber}@guardian.local",
                _passwordHasher.HashPassword(request.DocumentNumber),
                role.Id,
                guardianId: guardian.Id);

            await _guardianRepository.AddAsync(guardian);
            await _userRepository.AddAsync(user);
            await _guardianRepository.SaveChangesAsync();

            await _auditService.LogAsync("Created", "Guardian", guardian.Id.ToString(), null, $"{{\"DocumentNumber\":\"{guardian.DocumentNumber}\"}}");

            return guardian.Id;
        }
    }
}
