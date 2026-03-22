

namespace Transport.Domain.Entities
{
    public class SchoolDistrict
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public ICollection<Sector> Sectors { get; set; }
    }
}
