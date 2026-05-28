using MediatR;
using Transport.Application.Features.Backups.DTOs;

namespace Transport.Application.Features.Backups.Queries.GetBackupById
{
    public record GetBackupByIdQuery(Guid Id) : IRequest<BackupRecordResponseDto>;
}
