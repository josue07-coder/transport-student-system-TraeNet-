using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetDriversPerformanceReport
{
    public class GetDriversPerformanceReportHandler : IRequestHandler<GetDriversPerformanceReportQuery, List<DriverPerformanceReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetDriversPerformanceReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<List<DriverPerformanceReportDto>> Handle(GetDriversPerformanceReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetDriversPerformanceAsync(request.StartDate, request.EndDate, cancellationToken);
        }
    }
}
