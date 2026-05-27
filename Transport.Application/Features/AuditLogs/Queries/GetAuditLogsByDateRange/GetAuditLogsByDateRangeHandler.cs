using MediatR;
using Transport.Application.Features.AuditLogs.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByDateRange
{
    public class GetAuditLogsByDateRangeHandler : IRequestHandler<GetAuditLogsByDateRangeQuery, List<AuditLogResponseDto>>
    {
        private readonly IAuditLogRepository _repository;

        public GetAuditLogsByDateRangeHandler(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AuditLogResponseDto>> Handle(GetAuditLogsByDateRangeQuery request, CancellationToken cancellationToken)
        {
            if (request.EndDate.Date < request.StartDate.Date)
                throw new DomainException("La fecha final no puede ser menor que la fecha inicial");

            var logs = await _repository.GetByDateRangeAsync(request.StartDate, request.EndDate);
            return logs.Select(AuditLogMappings.ToResponseDto).ToList();
        }
    }
}
