using Transport.Application.Features.AuditLogs.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.AuditLogs.Queries
{
    internal static class AuditLogMappings
    {
        public static AuditLogResponseDto ToResponseDto(AuditLog auditLog)
        {
            return new AuditLogResponseDto
            {
                Id = auditLog.Id,
                UserId = auditLog.UserId,
                Username = auditLog.Username,
                Action = auditLog.Action,
                EntityName = auditLog.EntityName,
                EntityId = auditLog.EntityId,
                IpAddress = auditLog.IpAddress,
                UserAgent = auditLog.UserAgent,
                CorrelationId = auditLog.CorrelationId,
                TraceId = auditLog.TraceId,
                RequestPath = auditLog.RequestPath,
                HttpMethod = auditLog.HttpMethod,
                CreatedAt = auditLog.CreatedAt
            };
        }
    }
}
