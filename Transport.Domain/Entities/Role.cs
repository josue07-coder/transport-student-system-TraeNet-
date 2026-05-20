using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Role() { } // EF

        public Role(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Role name is required");

            Name = name;
            Description = description;
        }

        public void Update(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Role name is required");

            Name = name;
            Description = description;
        }

        public void AssignPermission(Guid permissionId)
        {
            if (permissionId == Guid.Empty)
                throw new DomainException("Permission is required");

            if (_rolePermissions.Any(rolePermission => rolePermission.PermissionId == permissionId))
                throw new DomainException("Permission already assigned to role");

            _rolePermissions.Add(new RolePermission(Id, permissionId));
        }

        public void RemovePermission(Guid permissionId)
        {
            var rolePermission = _rolePermissions.FirstOrDefault(item => item.PermissionId == permissionId);
            if (rolePermission is not null)
                _rolePermissions.Remove(rolePermission);
        }
    }
}
