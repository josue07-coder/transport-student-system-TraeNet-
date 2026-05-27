using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByEntity
{
    public record GetAuditLogsByEntityQuery(string EntityName, string EntityId) : IRequest<List<AuditLogResponseDto>>;
}
