using MediatR;

namespace Transport.Application.Features.Roles.Commands.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommand : IRequest<Unit>
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public AssignPermissionToRoleCommand(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
