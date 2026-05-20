using MediatR;
using Transport.Application.Features.Permissions.DTOs;

namespace Transport.Application.Features.Permissions.Queries.GetAllPermissions
{
    public class GetAllPermissionsQuery : IRequest<List<PermissionResponseDto>>
    {
    }
}
