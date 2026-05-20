using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Entities
{
    public class TransportAssistant : BaseEntity, IActivatable
    {
        public DocumentType DocumentType { get; private set; }
        public string DocumentNumber { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public PhoneNumber Phone { get; private set; }
        public Address Address { get; private set; }
        public Email? Email { get; private set; }
        public string? PhotoUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        private TransportAssistant() { } // EF Core

        public TransportAssistant(
            DocumentType documentType,
            string documentNumber,
            string firstName,
            string lastName,
            PhoneNumber phone,
            Address address,
            Email? email,
            string? photoUrl = null)
        {
            UpdateDocument(documentType, documentNumber);
            SetName(firstName, lastName);
            UpdateContact(phone, address, email);
            UpdatePhoto(photoUrl);
        }

        public void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("El apellido es obligatorio");

            FirstName = firstName;
            LastName = lastName;
        }

        public void UpdateDocument(DocumentType documentType, string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new DomainException("El numero de documento es obligatorio");

            DocumentType = documentType;
            DocumentNumber = documentNumber;
        }

        public void UpdateContact(PhoneNumber phone, Address address, Email? email)
        {
            Phone = phone ?? throw new DomainException("El telefono es obligatorio");
            Address = address ?? throw new DomainException("La direccion es obligatoria");
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
