using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Me.Commands.UpdateProfilePhoto
{
    public class UpdateProfilePhotoHandler : IRequestHandler<UpdateProfilePhotoCommand, Unit>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public UpdateProfilePhotoHandler(
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IAuditService auditService)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(UpdateProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            var user = await _userRepository.GetByIdWithLinkedProfilesAsync(userId)
                ?? throw new DomainException("Usuario no encontrado");

            var normalizedProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl)
                ? null
                : request.ProfileImageUrl.Trim();

            var oldValues = string.IsNullOrWhiteSpace(user.ProfileImageUrl)
                ? null
                : $"{{\"ProfileImageUrl\":\"{user.ProfileImageUrl}\"}}";

            user.UpdatePhoto(normalizedProfileImageUrl);
            user.Guardian?.UpdatePhoto(normalizedProfileImageUrl);
            user.Driver?.UpdatePhoto(normalizedProfileImageUrl);
            user.TransportAssistant?.UpdatePhoto(normalizedProfileImageUrl);

            await _userRepository.SaveChangesAsync();

            var action = normalizedProfileImageUrl is null
                ? "ProfilePhotoRemoved"
                : "ProfilePhotoUpdated";
            var newValues = normalizedProfileImageUrl is null
                ? null
                : $"{{\"ProfileImageUrl\":\"{normalizedProfileImageUrl}\"}}";

            await _auditService.LogAsync(action, "User", user.Id.ToString(), oldValues, newValues);

            return Unit.Value;
        }
    }
}
