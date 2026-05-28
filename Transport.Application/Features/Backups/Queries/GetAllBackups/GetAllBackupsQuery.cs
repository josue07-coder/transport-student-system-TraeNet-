using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Backups.DTOs;

namespace Transport.Application.Features.Backups.Queries.GetAllBackups
{
    public class GetAllBackupsQuery : PaginationRequest, IRequest<PaginatedResponse<BackupRecordResponseDto>>
    {
    }
}
