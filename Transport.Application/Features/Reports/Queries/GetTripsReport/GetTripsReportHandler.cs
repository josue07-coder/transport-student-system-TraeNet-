using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetTripsReport
{
    public class GetTripsReportHandler : IRequestHandler<GetTripsReportQuery, List<TripReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetTripsReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<List<TripReportDto>> Handle(GetTripsReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetTripsReportAsync(request.StartDate, request.EndDate, cancellationToken);
        }
    }
}
