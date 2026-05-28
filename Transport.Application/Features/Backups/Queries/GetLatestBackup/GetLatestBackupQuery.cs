using MediatR;
using Transport.Application.Features.Backups.DTOs;

namespace Transport.Application.Features.Backups.Queries.GetLatestBackup
{
    public record GetLatestBackupQuery : IRequest<BackupRecordResponseDto?>;
}
