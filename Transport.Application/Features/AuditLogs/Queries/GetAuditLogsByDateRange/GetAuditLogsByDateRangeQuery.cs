using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByDateRange
{
    public record GetAuditLogsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<AuditLogResponseDto>>;
}
