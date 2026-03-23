using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class AuditLog: BaseEntity
    {
        public string Action { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }
}
