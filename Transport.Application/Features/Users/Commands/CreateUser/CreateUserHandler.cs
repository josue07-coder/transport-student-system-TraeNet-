using System.Text.Json;
using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private static readonly HashSet<string> AllowedAdministrativeRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin",
            "Supervisor"
        };

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly IAuditService _auditService;

        public CreateUserHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasherService passwordHasherService,
            ISystemSettingService systemSettingService,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasherService = passwordHasherService;
            _systemSettingService = systemSettingService;
            _auditService = auditService;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
                throw new DomainException("El username ya está registrado");

            if (await _userRepository.ExistsByEmailAsync(request.Email))
                throw new DomainException("El email ya está registrado");

            var role = await _roleRepository.GetByIdAsync(request.RoleId)
                ?? throw new DomainException("Rol no encontrado");

            if (!AllowedAdministrativeRoles.Contains(role.Name))
                throw new DomainException("Solo se pueden crear usuarios con rol Admin o Supervisor desde este endpoint");

            var minimumPasswordLength = await _systemSettingService.GetIntAsync("Security.PasswordMinLength", 6);
            if (request.Password.Length < minimumPasswordLength)
                throw new DomainException($"La contraseña debe tener al menos {minimumPasswordLength} caracteres");

            var user = new User(
                request.Username,
                request.Name,
                request.Email,
                _passwordHasherService.HashPassword(request.Password),
                role.Id,
                profileImageUrl: request.ProfileImageUrl);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var newValues = JsonSerializer.Serialize(new
            {
                user.Username,
                user.Name,
                user.Email,
                user.RoleId,
                user.IsActive,
                user.ProfileImageUrl
            });

            await _auditService.LogAsync("UserCreated", "User", user.Id.ToString(), null, newValues);

            return user.Id;
        }
    }
}
