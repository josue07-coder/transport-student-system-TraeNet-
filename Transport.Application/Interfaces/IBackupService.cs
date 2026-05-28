using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IBackupService
    {
        Task<BackupRecord> CreateManualBackupAsync(Guid userId, CancellationToken cancellationToken);
        Task<BackupRecord?> GetLatestBackupAsync(CancellationToken cancellationToken);
    }
}
