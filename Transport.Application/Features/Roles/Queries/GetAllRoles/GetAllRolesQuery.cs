using MediatR;
using Transport.Application.Features.Roles.DTOs;

namespace Transport.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<List<RoleResponseDto>>
    {
    }
}
