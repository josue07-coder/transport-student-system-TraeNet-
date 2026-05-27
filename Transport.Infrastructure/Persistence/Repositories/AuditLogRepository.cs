using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }

        public async Task<PaginatedResponse<AuditLog>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.AuditLogs
                .OrderByDescending(log => log.CreatedAt)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AuditLog>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<AuditLog>> GetByUserAsync(Guid userId)
        {
            return await _context.AuditLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByEntityAsync(string entityName, string entityId)
        {
            return await _context.AuditLogs
                .Where(log => log.EntityName == entityName && log.EntityId == entityId)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByActionAsync(string action)
        {
            return await _context.AuditLogs
                .Where(log => log.Action == action)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.AuditLogs
                .Where(log => log.CreatedAt.Date >= startDate.Date && log.CreatedAt.Date <= endDate.Date)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
