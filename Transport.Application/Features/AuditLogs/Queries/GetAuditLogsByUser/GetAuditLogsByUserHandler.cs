using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByUser
{
    public class GetAuditLogsByUserHandler : IRequestHandler<GetAuditLogsByUserQuery, List<AuditLogResponseDto>>
    {
        private readonly IAuditLogRepository _repository;

        public GetAuditLogsByUserHandler(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AuditLogResponseDto>> Handle(GetAuditLogsByUserQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repository.GetByUserAsync(request.UserId);
            return logs.Select(AuditLogMappings.ToResponseDto).ToList();
        }
    }
}
