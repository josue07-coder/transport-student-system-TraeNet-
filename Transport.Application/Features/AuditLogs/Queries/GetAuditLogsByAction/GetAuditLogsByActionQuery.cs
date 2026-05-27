using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByAction
{
    public record GetAuditLogsByActionQuery(string Action) : IRequest<List<AuditLogResponseDto>>;
}
