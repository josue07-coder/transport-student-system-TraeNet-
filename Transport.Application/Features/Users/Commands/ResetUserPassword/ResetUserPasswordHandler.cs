using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Users.Commands.ResetUserPassword
{
    public class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public ResetUserPasswordHandler(IUserRepository userRepository, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId)
                ?? throw new DomainException("Usuario no encontrado");

            user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
