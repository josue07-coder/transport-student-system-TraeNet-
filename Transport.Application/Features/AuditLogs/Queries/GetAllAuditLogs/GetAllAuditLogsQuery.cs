using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.AuditLogs.DTOs;

namespace Transport.Application.Features.AuditLogs.Queries.GetAllAuditLogs
{
    public class GetAllAuditLogsQuery : PaginationRequest, IRequest<PaginatedResponse<AuditLogResponseDto>>
    {
    }
}
