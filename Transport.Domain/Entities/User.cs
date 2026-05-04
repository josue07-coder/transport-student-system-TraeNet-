using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class User : BaseEntity, IActivatable
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public string? ProfileImageUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Guid RoleId { get; private set; }

        private User() { } // EF

        public User(string name, string email, string passwordHash, Guid roleId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password is required");

            if (roleId == Guid.Empty)
                throw new DomainException("Role is required");

            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId;
        }

        public void UpdateProfile(string name, string? imageUrl)
        {
            Name = name;
            ProfileImageUrl = imageUrl;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}