using MediatR;
using Transport.Application.Features.Users.DTOs;

namespace Transport.Application.Features.Users.Queries.GetUsersByRole
{
    public class GetUsersByRoleQuery : IRequest<List<UserResponseDto>>
    {
        public Guid RoleId { get; set; }

        public GetUsersByRoleQuery(Guid roleId)
        {
            RoleId = roleId;
        }
    }
}
