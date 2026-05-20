namespace Transport.Domain.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = null!;

        public Guid PermissionId { get; private set; }
        public Permission Permission { get; private set; } = null!;

        private RolePermission() { } // EF

        public RolePermission(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
