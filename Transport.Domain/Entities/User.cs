using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class User: BaseEntity
    {
        
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ProfileImageUrl { get; set; }

        public Guid  RolId { get; set; }
        public Role Role { get; set; }
        
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
