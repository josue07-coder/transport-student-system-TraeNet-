using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class SchoolDistrict : BaseEntity
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public Address Address { get; private set; }

        private readonly List<Sector> _sectors = new();
        public IReadOnlyCollection<Sector> Sectors => _sectors.AsReadOnly();

        private SchoolDistrict() { } // EF Core

        public SchoolDistrict(string name, string code, Address address, string? description = null)
        {
            SetName(name);
            SetCode(code);
            Address = address ?? throw new DomainException("Address is required");
            Description = description ?? string.Empty;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("District name is required");

            Name = name;
        }

        public void SetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("District code is required");

            Code = code;
        }

        public void UpdateDescription(string? description)
        {
            Description = description ?? string.Empty;
        }
        public void UpdateAddress(Address address)
        {
            Address = address ?? throw new DomainException("Address is required");
        }
    }
}