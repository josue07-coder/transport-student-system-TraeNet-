using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Entities
{
    public class Driver : BaseEntity, IActivatable
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DocumentType DocumentType { get; private set; }
        public string DocumentNumber { get; private set; }
        public LicenseNumber LicenseNumber { get; private set; }
        public PhoneNumber Phone { get; private set; }
        public Address Address { get; private set; }
        public Email? Email { get; private set; }
        public string? PhotoUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Driver() { } // EF Core

        public Driver(
            string firstName,
            string lastName,
            DocumentType documentType,
            string documentNumber,
            LicenseNumber license,
            PhoneNumber phone,
            Address address,
            Email? email)
        {
            SetName(firstName, lastName);
            SetDocument(documentType, documentNumber);
            UpdateLicense(license);
            UpdateContact(phone, address, email);
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

        public void SetDocument(DocumentType documentType, string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new DomainException("Document number is required");

            DocumentType = documentType;
            DocumentNumber = documentNumber;
        }

        public void UpdateLicense(LicenseNumber license)
        {
            LicenseNumber = license ?? throw new DomainException("License number is required");
        }

        public void UpdateContact(PhoneNumber phone, Address address, Email? email)
        {
            Phone = phone ?? throw new DomainException("Phone is required");
            Address = address ?? throw new DomainException("Address is required");
            Email = email;
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
