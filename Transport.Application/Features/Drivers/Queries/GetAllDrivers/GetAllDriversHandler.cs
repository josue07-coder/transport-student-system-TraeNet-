using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Drivers.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Drivers.Queries.GetAllDrivers
{
    public class GetAllDriversHandler : IRequestHandler<GetAllDriversQuery, PaginatedResponse<DriverResponseDto>>
    {
        private readonly IDriverRepository _repository;

        public GetAllDriversHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<DriverResponseDto>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
        {
            var drivers = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = drivers.Items.Select(DriverMappings.ToResponseDto).ToList();

            return new PaginatedResponse<DriverResponseDto>(items, drivers.TotalCount, drivers.PageNumber, drivers.PageSize);
        }
    }
}
