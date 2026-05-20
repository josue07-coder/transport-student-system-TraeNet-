using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class User : BaseEntity, IActivatable
    {
        public string Username { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public string? ProfileImageUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = null!;

        public Guid? GuardianId { get; private set; }
        public Guardian? Guardian { get; private set; }

        public Guid? DriverId { get; private set; }
        public Driver? Driver { get; private set; }

        public Guid? TransportAssistantId { get; private set; }
        public TransportAssistant? TransportAssistant { get; private set; }

        public DateTime? LastLoginAt { get; private set; }

        private User() { } // EF

        public User(
            string username,
            string name,
            string email,
            string passwordHash,
            Guid roleId,
            Guid? guardianId = null,
            Guid? driverId = null,
            Guid? transportAssistantId = null,
            string? profileImageUrl = null)
        {
            SetUsername(username);
            UpdateProfile(name, profileImageUrl);
            SetEmail(email);
            ChangePassword(passwordHash);

            if (roleId == Guid.Empty)
                throw new DomainException("Role is required");

            RoleId = roleId;
            GuardianId = guardianId;
            DriverId = driverId;
            TransportAssistantId = transportAssistantId;
        }

        public void SetUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException("Username is required");

            Username = username;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required");

            Email = email;
        }

        public void UpdateProfile(string name, string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");

            Name = name;
            ProfileImageUrl = imageUrl;
        }

        public void UpdatePhoto(string? imageUrl)
        {
            ProfileImageUrl = imageUrl;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException("Password is required");

            PasswordHash = newPasswordHash;
        }

        public void MarkLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
