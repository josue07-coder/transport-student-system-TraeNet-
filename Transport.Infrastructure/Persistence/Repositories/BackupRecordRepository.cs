using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class BackupRecordRepository : IBackupRecordRepository
    {
        private readonly AppDbContext _context;

        public BackupRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BackupRecord backupRecord)
        {
            await _context.BackupRecords.AddAsync(backupRecord);
        }

        public Task<BackupRecord?> GetByIdAsync(Guid id)
        {
            return _context.BackupRecords.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaginatedResponse<BackupRecord>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.BackupRecords
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<BackupRecord>(items, totalCount, pageNumber, pageSize);
        }

        public Task<BackupRecord?> GetLatestAsync()
        {
            return _context.BackupRecords
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
