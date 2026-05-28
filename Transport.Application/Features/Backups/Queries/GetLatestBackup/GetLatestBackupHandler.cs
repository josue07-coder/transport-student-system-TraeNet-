using MediatR;
using Transport.Application.Features.Backups.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Backups.Queries.GetLatestBackup
{
    public class GetLatestBackupHandler : IRequestHandler<GetLatestBackupQuery, BackupRecordResponseDto?>
    {
        private readonly IBackupService _backupService;

        public GetLatestBackupHandler(IBackupService backupService)
        {
            _backupService = backupService;
        }

        public async Task<BackupRecordResponseDto?> Handle(GetLatestBackupQuery request, CancellationToken cancellationToken)
        {
            var backup = await _backupService.GetLatestBackupAsync(cancellationToken);
            return backup is null ? null : BackupMappings.ToResponseDto(backup);
        }
    }
}
