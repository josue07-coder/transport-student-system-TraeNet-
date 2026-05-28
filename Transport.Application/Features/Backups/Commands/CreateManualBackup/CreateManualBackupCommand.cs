using MediatR;
using Transport.Application.Features.Backups.DTOs;

namespace Transport.Application.Features.Backups.Commands.CreateManualBackup
{
    public record CreateManualBackupCommand : IRequest<BackupRecordResponseDto>;
}
