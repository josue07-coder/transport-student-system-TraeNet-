using MediatR;
using Transport.Application.Features.Roles.DTOs;

namespace Transport.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleCommand : IRequest<RoleResponseDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
