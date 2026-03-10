using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class AuditLogs
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }
}
