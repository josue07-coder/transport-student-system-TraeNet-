using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class BackupRecord : BaseEntity
    {
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public long? FileSizeBytes { get; private set; }
        public BackupStatus Status { get; private set; }
        public BackupType Type { get; private set; }
        public string? ErrorMessage { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        private BackupRecord() { } // EF

        public BackupRecord(string fileName, string filePath, BackupType type, Guid createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("El nombre del archivo de backup es requerido");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new DomainException("La ruta del archivo de backup es requerida");

            if (createdByUserId == Guid.Empty)
                throw new DomainException("El usuario creador del backup es requerido");

            FileName = fileName.Trim();
            FilePath = filePath.Trim();
            Type = type;
            CreatedByUserId = createdByUserId;
            Status = BackupStatus.Pending;
        }

        public void MarkInProgress()
        {
            Status = BackupStatus.InProgress;
            ErrorMessage = null;
            SetUpdated();
        }

        public void MarkCompleted(long fileSizeBytes)
        {
            if (fileSizeBytes < 0)
                throw new DomainException("El tamaño del backup no puede ser negativo");

            FileSizeBytes = fileSizeBytes;
            Status = BackupStatus.Completed;
            ErrorMessage = null;
            CompletedAt = DateTime.UtcNow;
            SetUpdated();
        }

        public void MarkFailed(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new DomainException("El mensaje de error del backup es requerido");

            Status = BackupStatus.Failed;
            ErrorMessage = errorMessage.Trim();
            CompletedAt = DateTime.UtcNow;
            SetUpdated();
        }
    }
}
