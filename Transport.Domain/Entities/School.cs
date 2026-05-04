using Transport.Domain.Common;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Entities
{
    public class School : BaseEntity, IActivatable
    {
        public string Name { get; private set; }
        public string DirectorName { get; private set; }
        public Email ContactEmail { get; private set; }
        public PhoneNumber ContactPhone { get; private set; }
        public string Description { get; private set; }
        public Address Address { get; private set; }
        public string? ProfileImageUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Guid SectorId { get; private set; }

        private School() { } // EF Core

        public School(
            string name,
            string directorName,
            Email contactEmail,
            PhoneNumber contactPhone,
            Address address,
            Guid sectorId,
            string? description = null,
            string? profileImageUrl = null)
        {
            SetName(name);
            SetDirectorName(directorName);

            ContactEmail = contactEmail ?? throw new DomainException("Email is required");
            ContactPhone = contactPhone ?? throw new DomainException("Phone is required");
            Address = address ?? throw new DomainException("Address is required");

            if (sectorId == Guid.Empty)
                throw new DomainException("Sector is required");

            SectorId = sectorId;
            Description = description;
            ProfileImageUrl = profileImageUrl;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("School name is required");

            Name = name;
        }

        public void SetDirectorName(string directorName)
        {
            if (string.IsNullOrWhiteSpace(directorName))
                throw new DomainException("Director name is required");

            DirectorName = directorName;
        }

        public void UpdateContact(Email email, PhoneNumber phone)
        {
            ContactEmail = email ?? throw new DomainException("Email is required");
            ContactPhone = phone ?? throw new DomainException("Phone is required");
        }

        public void UpdateAddress(Address address)
        {
            Address = address ?? throw new DomainException("Address is required");
        }

        public void UpdateProfile(string? description, string? profileImageUrl)
        {
            Description = description;
            ProfileImageUrl = profileImageUrl;
        }

        public void ChangeSector(Guid sectorId)
        {
            if (sectorId == Guid.Empty)
                throw new DomainException("Sector is required");

            SectorId = sectorId;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}