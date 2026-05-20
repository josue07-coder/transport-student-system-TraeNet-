using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Me.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public ChangePasswordHandler(
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new DomainException("Usuario no encontrado");

            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                throw new DomainException("La contraseña actual no es válida");

            user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
