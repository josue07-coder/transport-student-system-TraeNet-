using Transport.Domain.Enums;

namespace Transport.Application.Features.Backups.DTOs
{
    public class BackupRecordResponseDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long? FileSizeBytes { get; set; }
        public BackupStatus Status { get; set; }
        public BackupType Type { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
