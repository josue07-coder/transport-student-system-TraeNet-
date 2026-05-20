using MediatR;

namespace Transport.Application.Features.Roles.Commands.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleCommand : IRequest<Unit>
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public RemovePermissionFromRoleCommand(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
