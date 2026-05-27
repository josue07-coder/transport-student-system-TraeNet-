using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetDashboardReport
{
    public class GetDashboardReportHandler : IRequestHandler<GetDashboardReportQuery, DashboardReportDto>
    {
        private readonly IReportRepository _repository;

        public GetDashboardReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<DashboardReportDto> Handle(GetDashboardReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetDashboardAsync(cancellationToken);
        }
    }
}
