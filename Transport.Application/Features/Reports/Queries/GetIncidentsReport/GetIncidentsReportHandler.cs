using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetIncidentsReport
{
    public class GetIncidentsReportHandler : IRequestHandler<GetIncidentsReportQuery, List<IncidentReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetIncidentsReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<List<IncidentReportDto>> Handle(GetIncidentsReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetIncidentsReportAsync(request.StartDate, request.EndDate, cancellationToken);
        }
    }
}
