using Transport.Application.Interfaces;
using Transport.Domain.Entities;

namespace Transport.API.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            IAuditLogRepository repository,
            ICurrentUserService currentUserService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogAsync(
            string action,
            string entityName,
            string? entityId = null,
            string? oldValues = null,
            string? newValues = null)
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                var ipAddress = context?.Connection.RemoteIpAddress?.ToString();
                var userAgent = context?.Request.Headers.UserAgent.ToString();
                var correlationId = context?.TraceIdentifier;
                var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? correlationId;
                var requestPath = context?.Request.Path.Value;
                var httpMethod = context?.Request.Method;

                var auditLog = new AuditLog(
                    _currentUserService.UserId,
                    Truncate(_currentUserService.Username, 100),
                    Truncate(action, 50) ?? action,
                    Truncate(entityName, 150) ?? entityName,
                    Truncate(entityId, 100),
                    SanitizeAuditValues(oldValues),
                    SanitizeAuditValues(newValues),
                    Truncate(ipAddress, 100),
                    Truncate(userAgent, 500),
                    Truncate(correlationId, 100),
                    Truncate(traceId, 100),
                    Truncate(requestPath, 300),
                    Truncate(httpMethod, 20));

                await _repository.AddAsync(auditLog);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Audit logging failed for {Action} on {EntityName} {EntityId}", action, entityName, entityId);
            }
        }

        private static string? SanitizeAuditValues(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var sanitized = value;
            var sensitiveKeys = new[]
            {
                "password",
                "passwordHash",
                "currentPassword",
                "newPassword",
                "confirmPassword",
                "token",
                "jwt",
                "authorization"
            };

            foreach (var key in sensitiveKeys)
            {
                sanitized = System.Text.RegularExpressions.Regex.Replace(
                    sanitized,
                    $"(\"{key}\"\\s*:\\s*\")([^\"]*)(\")",
                    $"$1[REDACTED]$3",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return sanitized;
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value[..maxLength];
        }
    }
}
