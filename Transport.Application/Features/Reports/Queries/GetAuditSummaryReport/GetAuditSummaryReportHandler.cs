using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetAuditSummaryReport
{
    public class GetAuditSummaryReportHandler : IRequestHandler<GetAuditSummaryReportQuery, List<AuditSummaryReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetAuditSummaryReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<List<AuditSummaryReportDto>> Handle(GetAuditSummaryReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetAuditSummaryAsync(request.StartDate, request.EndDate, cancellationToken);
        }
    }
}
