using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetLowPresenceReport
{
    public class GetLowPresenceReportHandler : IRequestHandler<GetLowPresenceReportQuery, List<LowPresenceStudentReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetLowPresenceReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<List<LowPresenceStudentReportDto>> Handle(GetLowPresenceReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetLowPresenceReportAsync(
                request.StartDate,
                request.EndDate,
                request.MaximumPresencePercentage,
                cancellationToken);
        }
    }
}
