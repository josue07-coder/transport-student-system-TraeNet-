using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Users.Commands.DeactivateUser
{
    public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public DeactivateUserHandler(IUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Usuario no encontrado");

            user.Deactivate();
            await _userRepository.SaveChangesAsync();

            await _auditService.LogAsync("Deactivated", "User", user.Id.ToString(), $"{{\"Username\":\"{user.Username}\"}}");

            return Unit.Value;
        }
    }
}
