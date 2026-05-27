using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByEntity
{
    public class GetAuditLogsByEntityHandler : IRequestHandler<GetAuditLogsByEntityQuery, List<AuditLogResponseDto>>
    {
        private readonly IAuditLogRepository _repository;

        public GetAuditLogsByEntityHandler(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AuditLogResponseDto>> Handle(GetAuditLogsByEntityQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repository.GetByEntityAsync(request.EntityName, request.EntityId);
            return logs.Select(AuditLogMappings.ToResponseDto).ToList();
        }
    }
}
