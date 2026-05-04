using Transport.Domain.Common;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Entities
{
    public class Driver : BaseEntity, IActivatable
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public LicenseNumber LicenseNumber { get; private set; }
        public string? PhotoUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Driver() { } // EF Core

        public Driver(string firstName, string lastName, LicenseNumber license)
        {
            SetName(firstName, lastName);

            LicenseNumber = license ?? throw new DomainException("License number is required");
        }

        public void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name is required");

            FirstName = firstName;
            LastName = lastName;
        }

        public void UpdateLicense(LicenseNumber license)
        {
            LicenseNumber = license ?? throw new DomainException("License number is required");
        }
        public void UpdatePhoto(string? photoUrl)
        {
            PhotoUrl = photoUrl;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}