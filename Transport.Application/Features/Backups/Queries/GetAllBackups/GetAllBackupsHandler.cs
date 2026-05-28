using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Backups.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Backups.Queries.GetAllBackups
{
    public class GetAllBackupsHandler : IRequestHandler<GetAllBackupsQuery, PaginatedResponse<BackupRecordResponseDto>>
    {
        private readonly IBackupRecordRepository _repository;

        public GetAllBackupsHandler(IBackupRecordRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<BackupRecordResponseDto>> Handle(GetAllBackupsQuery request, CancellationToken cancellationToken)
        {
            var backups = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            return new PaginatedResponse<BackupRecordResponseDto>(
                backups.Items.Select(BackupMappings.ToResponseDto),
                backups.TotalCount,
                backups.PageNumber,
                backups.PageSize);
        }
    }
}
