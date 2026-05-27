using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetVehiclesUsageReport
{
    public class GetVehiclesUsageReportHandler : IRequestHandler<GetVehiclesUsageReportQuery, List<VehicleUsageReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetVehiclesUsageReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<List<VehicleUsageReportDto>> Handle(GetVehiclesUsageReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetVehiclesUsageAsync(request.StartDate, request.EndDate, cancellationToken);
        }
    }
}
