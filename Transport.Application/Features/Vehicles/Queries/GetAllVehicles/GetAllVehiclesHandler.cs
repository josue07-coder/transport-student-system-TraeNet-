using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Vehicles.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Vehicles.Queries.GetAllVehicles
{
    public class GetAllVehiclesHandler : IRequestHandler<GetAllVehiclesQuery, PaginatedResponse<VehicleResponseDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetAllVehiclesHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<VehicleResponseDto>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = vehicles.Items.Select(v => new VehicleResponseDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Capacity = v.Capacity,
                Status = v.Status
            }).ToList();

            return new PaginatedResponse<VehicleResponseDto>(items, vehicles.TotalCount, vehicles.PageNumber, vehicles.PageSize);
        }
    }
}
