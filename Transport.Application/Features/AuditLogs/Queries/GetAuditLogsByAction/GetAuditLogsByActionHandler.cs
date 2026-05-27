using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByAction
{
    public class GetAuditLogsByActionHandler : IRequestHandler<GetAuditLogsByActionQuery, List<AuditLogResponseDto>>
    {
        private readonly IAuditLogRepository _repository;

        public GetAuditLogsByActionHandler(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AuditLogResponseDto>> Handle(GetAuditLogsByActionQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repository.GetByActionAsync(request.Action);
            return logs.Select(AuditLogMappings.ToResponseDto).ToList();
        }
    }
}
