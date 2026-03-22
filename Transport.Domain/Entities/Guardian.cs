
using Transport.Domain.Enums;

namespace Transport.Domain.Entities
{
    public class Guardian
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }

        public Guid SectorId { get; set; }
        public Sector Sector { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
