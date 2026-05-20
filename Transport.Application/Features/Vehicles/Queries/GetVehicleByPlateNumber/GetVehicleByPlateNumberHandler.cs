using MediatR;
using Transport.Application.Features.Vehicles.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Vehicles.Queries.GetVehicleByPlateNumber
{
    public class GetVehicleByPlateNumberHandler : IRequestHandler<GetVehicleByPlateNumberQuery, VehicleDetailDto>
    {
        private readonly IVehicleRepository _repository;

        public GetVehicleByPlateNumberHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<VehicleDetailDto> Handle(GetVehicleByPlateNumberQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await _repository.GetByPlateNumberAsync(request.PlateNumber)
                ?? throw new DomainException("Vehiculo no encontrado");

            return new VehicleDetailDto
            {
                Id = vehicle.Id,
                PlateNumber = vehicle.PlateNumber,
                Capacity = vehicle.Capacity,
                Status = vehicle.Status
            };
        }
    }
}
