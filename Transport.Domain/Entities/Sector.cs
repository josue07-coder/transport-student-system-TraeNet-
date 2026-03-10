

namespace Transport.Domain.Entities
{
    public class Sector
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid DistrictId { get; set; }
        public District District { get; set; }
        public ICollection<School> Schools { get; set; }
        public ICollection<Stop> Stops { get; set; }

    }
}
