using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.AuditLogs.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.AuditLogs.Queries.GetAllAuditLogs
{
    public class GetAllAuditLogsHandler : IRequestHandler<GetAllAuditLogsQuery, PaginatedResponse<AuditLogResponseDto>>
    {
        private readonly IAuditLogRepository _repository;

        public GetAllAuditLogsHandler(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<AuditLogResponseDto>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = logs.Items.Select(AuditLogMappings.ToResponseDto).ToList();

            return new PaginatedResponse<AuditLogResponseDto>(items, logs.TotalCount, logs.PageNumber, logs.PageSize);
        }
    }
}
