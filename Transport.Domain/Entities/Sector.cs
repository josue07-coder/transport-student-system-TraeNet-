using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Sector : BaseEntity
    {
        public string Name { get; private set; }
        public string City { get; private set; }
        public string Province { get; private set; }

        public Guid SchoolDistrictId { get; private set; }

        private Sector() { } // EF Core

        public Sector(string name, string city, string province, Guid schoolDistrictId)
        {
            SetName(name);

            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City is required");

            if (string.IsNullOrWhiteSpace(province))
                throw new DomainException("Province is required");

            if (schoolDistrictId == Guid.Empty)
                throw new DomainException("School district is required");

            City = city;
            Province = province;
            SchoolDistrictId = schoolDistrictId;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Sector name is required");

            Name = name;
        }

        public void UpdateLocation(string city, string province)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City is required");

            if (string.IsNullOrWhiteSpace(province))
                throw new DomainException("Province is required");

            City = city;
            Province = province;
        }

        public void ChangeDistrict(Guid schoolDistrictId)
        {
            if (schoolDistrictId == Guid.Empty)
                throw new DomainException("School district is required");

            SchoolDistrictId = schoolDistrictId;
        }
    }
}