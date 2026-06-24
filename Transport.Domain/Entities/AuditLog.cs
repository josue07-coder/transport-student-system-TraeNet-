using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid? UserId { get; private set; }
        public string? Username { get; private set; }
        public string Action { get; private set; }
        public string EntityName { get; private set; }
        public string? EntityId { get; private set; }
        public string? OldValues { get; private set; }
        public string? NewValues { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public string? CorrelationId { get; private set; }
        public string? TraceId { get; private set; }
        public string? RequestPath { get; private set; }
        public string? HttpMethod { get; private set; }

        private AuditLog() { }

        public AuditLog(
            Guid? userId,
            string? username,
            string action,
            string entityName,
            string? entityId,
            string? oldValues,
            string? newValues,
            string? ipAddress,
            string? userAgent,
            string? correlationId = null,
            string? traceId = null,
            string? requestPath = null,
            string? httpMethod = null)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new DomainException("Audit action is required");

            if (string.IsNullOrWhiteSpace(entityName))
                throw new DomainException("Audit entity name is required");

            UserId = userId;
            Username = username;
            Action = action;
            EntityName = entityName;
            EntityId = entityId;
            OldValues = oldValues;
            NewValues = newValues;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            CorrelationId = correlationId;
            TraceId = traceId;
            RequestPath = requestPath;
            HttpMethod = httpMethod;
        }
    }
}
