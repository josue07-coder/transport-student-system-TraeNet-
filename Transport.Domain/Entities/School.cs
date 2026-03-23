using Transport.Domain.ValueObjects;
using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class School
    {
        public string Name { get; set; }
        public string DirectorName { get; set; }
        public string ContactEmail { get; set; }
        public String ContactPhone { get; set; }
        public String Description { get; set; }
        public Address Address { get; set; }
        public string? ProfileImageUrl { get; set; }

        public Guid SectorId { get; set; }
        public Sector Sector { get; set; }

        public ICollection<Student> Students { get; set; }
        public ICollection<Route> Routes { get; set; }
    }
}
