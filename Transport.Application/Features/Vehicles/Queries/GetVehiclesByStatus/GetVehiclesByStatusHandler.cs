using MediatR;
using Transport.Application.Features.Vehicles.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Vehicles.Queries.GetVehiclesByStatus
{
    public class GetVehiclesByStatusHandler : IRequestHandler<GetVehiclesByStatusQuery, List<VehicleResponseDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetVehiclesByStatusHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<VehicleResponseDto>> Handle(GetVehiclesByStatusQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetByStatusAsync(request.Status);

            return vehicles.Select(v => new VehicleResponseDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Capacity = v.Capacity,
                Status = v.Status
            }).ToList();
        }
    }
}
