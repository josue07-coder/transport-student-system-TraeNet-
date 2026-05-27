using MediatR;
using Transport.Application.Features.Auth.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuditService _auditService;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IJwtTokenService jwtTokenService,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _auditService = auditService;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username)
                ?? throw new DomainException("Usuario o contraseña inválidos");

            if (!user.IsActive)
                throw new DomainException("Usuario inactivo");

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new DomainException("Usuario o contraseña inválidos");

            user.MarkLogin();
            await _userRepository.SaveChangesAsync();

            var roleName = user.Role.Name;
            var token = _jwtTokenService.GenerateToken(user, roleName);

            await _auditService.LogAsync("Login", "User", user.Id.ToString(), null, $"{{\"Username\":\"{user.Username}\"}}");

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Name = user.Name,
                ProfileImageUrl = user.ProfileImageUrl,
                Role = roleName
            };
        }
    }
}
