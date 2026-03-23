using Transport.Shared.Common;
using Transport.Domain.Enums;

namespace Transport.Domain.Entities
{
    public class Guardian: BaseEntity
    {
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public String? PhotoUrl { get; set; }
        public Gender Gender { get; set; }

        public Guid SectorId { get; set; }
        public Sector Sector { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
