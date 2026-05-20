using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string Module { get; private set; }

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Permission() { } // EF

        public Permission(string name, string? description, string module)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Permission name is required");

            if (string.IsNullOrWhiteSpace(module))
                throw new DomainException("Permission module is required");

            Name = name;
            Description = description;
            Module = module;
        }
    }
}
