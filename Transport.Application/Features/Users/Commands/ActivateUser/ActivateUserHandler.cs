using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Users.Commands.ActivateUser
{
    public class ActivateUserHandler : IRequestHandler<ActivateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public ActivateUserHandler(IUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Usuario no encontrado");

            user.Activate();
            await _userRepository.SaveChangesAsync();

            await _auditService.LogAsync("Activated", "User", user.Id.ToString(), null, $"{{\"Username\":\"{user.Username}\"}}");

            return Unit.Value;
        }
    }
}
