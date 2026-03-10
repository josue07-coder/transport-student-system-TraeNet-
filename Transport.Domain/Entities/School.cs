

namespace Transport.Domain.Entities
{
    public class School
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public Guid SectorId { get; set; }

        public Sector Sector { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
