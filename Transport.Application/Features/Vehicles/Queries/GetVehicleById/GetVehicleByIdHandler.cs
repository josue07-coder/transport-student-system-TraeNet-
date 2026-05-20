using MediatR;
using Transport.Application.Features.Vehicles.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Vehicles.Queries.GetVehicleById
{
    public class GetVehicleByIdHandler : IRequestHandler<GetVehicleByIdQuery, VehicleDetailDto>
    {
        private readonly IVehicleRepository _repository;

        public GetVehicleByIdHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<VehicleDetailDto> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await _repository.GetByIdAsync(request.Id)
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
