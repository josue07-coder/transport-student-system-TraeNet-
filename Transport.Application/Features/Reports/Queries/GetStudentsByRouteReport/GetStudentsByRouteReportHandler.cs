using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Reports.Queries.GetStudentsByRouteReport
{
    public class GetStudentsByRouteReportHandler : IRequestHandler<GetStudentsByRouteReportQuery, StudentsByRouteReportDto>
    {
        private readonly IReportRepository _repository;

        public GetStudentsByRouteReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<StudentsByRouteReportDto> Handle(GetStudentsByRouteReportQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetStudentsByRouteAsync(request.RouteId, cancellationToken);
        }
    }
}
