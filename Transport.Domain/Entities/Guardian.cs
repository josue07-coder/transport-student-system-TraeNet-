using System.Xml.Linq;
using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Entities
{
    public class Guardian : BaseEntity
    {
        public DocumentType DocumentType { get; private set; }
        public string DocumentNumber { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public PhoneNumber Phone { get; private set; }
        public Address Address { get; private set; }

        public string? PhotoUrl { get; private set; }
        public Gender Gender { get; private set; }

        public Guid SectorId { get; private set; }

        private Guardian() { } // EF Core

        public Guardian(
            DocumentType documentType,
            string documentNumber,
            string firstName,
            string lastName,
            PhoneNumber phone,
            Address address,
            Gender gender,
            Guid sectorId,
            string? photoUrl = null)
        {
            SetDocument(documentType, documentNumber);
            SetName(firstName, lastName);

            Phone = phone ?? throw new DomainException("Phone is required");
            Address = address ?? throw new DomainException("Address is required");

            if (sectorId == Guid.Empty)
                throw new DomainException("Sector is required");

            Gender = gender;
            SectorId = sectorId;
            PhotoUrl = photoUrl;
        }

        public void SetDocument(DocumentType type, string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new DomainException("Document number is required");

            DocumentType = type;
            DocumentNumber = number;
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

        public void UpdateContact(PhoneNumber phone, Address address)
        {
            Phone = phone ?? throw new DomainException("Phone is required");
            Address = address ?? throw new DomainException("Address is required");
        }

        public void UpdatePhoto(string? photoUrl)
        {
            PhotoUrl = photoUrl;
        }

        public void ChangeSector(Guid sectorId)
        {
            if (sectorId == Guid.Empty)
                throw new DomainException("Sector is required");

            SectorId = sectorId;
        }
    }
}