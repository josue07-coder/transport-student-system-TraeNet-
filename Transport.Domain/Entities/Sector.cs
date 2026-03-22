

namespace Transport.Domain.Entities
{
    public class Sector
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid SchooDistrictId { get; set; }
        public SchoolDistrict SchoolDistrict { get; set; }

        public Guid MunicipalityId { get; set; }
        public Municipality Municipality { get; set; }

        public ICollection<School> Schools { get; set; }
        public ICollection<Stop> Stops { get; set; } 
        public ICollection<Guardian> Guardians { get; set; } 

    }
}
