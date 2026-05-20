using MediatR;
using Transport.Application.Features.Me.DTOs;

namespace Transport.Application.Features.Me.Commands.UpdateMeProfile
{
    public class UpdateMeProfileCommand : IRequest<MeProfileDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }
}
