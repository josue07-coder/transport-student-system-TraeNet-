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

        private Role() { } // EF

        public Role(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Role name is required");

            Name = name;
            Description = description;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }
    }
}