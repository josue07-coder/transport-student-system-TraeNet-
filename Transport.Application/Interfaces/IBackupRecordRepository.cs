using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IBackupRecordRepository
    {
        Task AddAsync(BackupRecord backupRecord);
        Task<BackupRecord?> GetByIdAsync(Guid id);
        Task<PaginatedResponse<BackupRecord>> GetPagedAsync(int pageNumber, int pageSize);
        Task<BackupRecord?> GetLatestAsync();
        Task SaveChangesAsync();
    }
}
