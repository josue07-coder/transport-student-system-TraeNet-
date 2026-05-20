using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Me.Commands.UpdateProfilePhoto
{
    public class UpdateProfilePhotoHandler : IRequestHandler<UpdateProfilePhotoCommand, Unit>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public UpdateProfilePhotoHandler(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(UpdateProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            var user = await _userRepository.GetByIdWithLinkedProfilesAsync(userId)
                ?? throw new DomainException("Usuario no encontrado");

            user.UpdatePhoto(request.ProfileImageUrl);
            user.Guardian?.UpdatePhoto(request.ProfileImageUrl);
            user.Driver?.UpdatePhoto(request.ProfileImageUrl);
            user.TransportAssistant?.UpdatePhoto(request.ProfileImageUrl);

            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
