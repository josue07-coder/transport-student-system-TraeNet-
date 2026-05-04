using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Guardian : BaseEntity, IActivatable
    {
        public DocumentType DocumentType { get; private set; }
        public string DocumentNumber { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public string Phone { get; private set; }
        public string Address { get; private set; }

        public string? PhotoUrl { get; private set; }
        public Gender Gender { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Guid SectorId { get; private set; }

        private readonly List<Student> _students = new();
        public IReadOnlyCollection<Student> Students => _students.AsReadOnly();

        private Guardian() { } // EF

        public Guardian(
            DocumentType documentType,
            string documentNumber,
            string firstName,
            string lastName,
            string phone,
            string address,
            Gender gender,
            Guid sectorId,
            string? photoUrl = null)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new DomainException("Document number is required");

            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name is required");

            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Phone is required");

            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Address is required");

            if (sectorId == Guid.Empty)
                throw new DomainException("Sector is required");

            DocumentType = documentType;
            DocumentNumber = documentNumber;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Address = address;
            Gender = gender;
            SectorId = sectorId;
            PhotoUrl = photoUrl;
        }

        public void UpdateContact(string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Phone is required");

            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Address is required");

            Phone = phone;
            Address = address;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}