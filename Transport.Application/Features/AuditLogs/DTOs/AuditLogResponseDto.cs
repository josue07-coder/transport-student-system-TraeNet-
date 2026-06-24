namespace Transport.Application.Features.AuditLogs.DTOs
{
    public class AuditLogResponseDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? TraceId { get; set; }
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
