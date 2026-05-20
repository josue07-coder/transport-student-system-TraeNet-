using MediatR;

namespace Transport.Application.Features.Me.Commands.UpdateProfilePhoto
{
    public class UpdateProfilePhotoCommand : IRequest<Unit>
    {
        public string ProfileImageUrl { get; set; } = string.Empty;
    }
}
