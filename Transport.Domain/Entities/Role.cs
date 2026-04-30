using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        private Role() { } // EF Core

        public Role(string name, string? description = null)
        {
            SetName(name);
            Description = description ?? string.Empty;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Role name is required");

            Name = name;
        }

        public void UpdateDescription(string? description)
        {
            Description = description ?? string.Empty;
        }
    }
}