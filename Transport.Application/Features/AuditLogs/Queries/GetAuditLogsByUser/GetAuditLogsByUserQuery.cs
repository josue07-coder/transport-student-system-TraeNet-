using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByUser
{
    public record GetAuditLogsByUserQuery(Guid UserId) : IRequest<List<AuditLogResponseDto>>;
}
