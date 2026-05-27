using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Users.Commands.ResetUserPassword
{
    public class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IAuditService _auditService;

        public ResetUserPasswordHandler(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId)
                ?? throw new DomainException("Usuario no encontrado");

            user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _userRepository.SaveChangesAsync();

            await _auditService.LogAsync("PasswordChanged", "User", user.Id.ToString(), null, "{\"Reset\":\"Admin\"}");

            return Unit.Value;
        }
    }
}
