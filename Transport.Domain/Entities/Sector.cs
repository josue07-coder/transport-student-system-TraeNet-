using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class Sector: BaseEntity
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Province { get; set; }

        public Guid SchooDistrictId { get; set; }
        public SchoolDistrict SchoolDistrict { get; set; }
        
        public ICollection<School> Schools { get; set; }
        public ICollection<Stop> Stops { get; set; } 
        public ICollection<Guardian> Guardians { get; set; } 

    }
}
