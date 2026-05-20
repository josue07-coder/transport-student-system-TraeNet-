using MediatR;
using Transport.Application.Features.Users.DTOs;

namespace Transport.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<UserDetailDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
