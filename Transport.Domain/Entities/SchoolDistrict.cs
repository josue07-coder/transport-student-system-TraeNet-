using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class SchoolDistrict : BaseEntity
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }

        private SchoolDistrict() { } // EF Core

        public SchoolDistrict(string name, string code, string? description = null)
        {
            SetName(name);
            SetCode(code);
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
    }
}