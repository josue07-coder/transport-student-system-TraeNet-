using Transport.Domain.Entities;

namespace Transport.Application.Features.Backups.DTOs
{
    internal static class BackupMappings
    {
        public static BackupRecordResponseDto ToResponseDto(BackupRecord backupRecord)
        {
            return new BackupRecordResponseDto
            {
                Id = backupRecord.Id,
                FileName = backupRecord.FileName,
                FilePath = backupRecord.FilePath,
                FileSizeBytes = backupRecord.FileSizeBytes,
                Status = backupRecord.Status,
                Type = backupRecord.Type,
                ErrorMessage = backupRecord.ErrorMessage,
                CreatedByUserId = backupRecord.CreatedByUserId,
                CreatedAt = backupRecord.CreatedAt,
                CompletedAt = backupRecord.CompletedAt
            };
        }
    }
}
