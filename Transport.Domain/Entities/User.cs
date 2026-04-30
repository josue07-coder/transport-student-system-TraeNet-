using Transport.Domain.Common;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string? ProfileImageUrl { get; private set; }

        public Guid RoleId { get; private set; }

        private User() { } // EF Core

        public User(string name, Email email, string passwordHash, Guid roleId)
        {
            SetName(name);

            Email = email ?? throw new DomainException("Email is required");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password is required");

            if (roleId == Guid.Empty)
                throw new DomainException("Role is required");

            PasswordHash = passwordHash;
            RoleId = roleId;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");

            Name = name;
        }

        public void UpdateEmail(Email email)
        {
            Email = email ?? throw new DomainException("Email is required");
        }

        public void UpdatePassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password is required");

            PasswordHash = passwordHash;
        }

        public void UpdateProfileImage(string? imageUrl)
        {
            ProfileImageUrl = imageUrl;
        }

        public void ChangeRole(Guid roleId)
        {
            if (roleId == Guid.Empty)
                throw new DomainException("Role is required");

            RoleId = roleId;
        }
    }
}