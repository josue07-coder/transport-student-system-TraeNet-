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

                var auditLog = new AuditLog(
                    _currentUserService.UserId,
                    _currentUserService.Username,
                    action,
                    entityName,
                    entityId,
                    oldValues,
                    newValues,
                    ipAddress,
                    userAgent);

                await _repository.AddAsync(auditLog);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Audit logging failed for {Action} on {EntityName} {EntityId}", action, entityName, entityId);
            }
        }
    }
}
