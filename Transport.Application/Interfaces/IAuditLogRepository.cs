using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<PaginatedResponse<AuditLog>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<AuditLog>> GetByUserAsync(Guid userId);
        Task<List<AuditLog>> GetByEntityAsync(string entityName, string entityId);
        Task<List<AuditLog>> GetByActionAsync(string action);
        Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task SaveChangesAsync();
    }
}
