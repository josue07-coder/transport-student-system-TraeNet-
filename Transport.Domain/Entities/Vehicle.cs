using Transport.Domain.Common;
using Transport.Domain.Exceptions;
using Transport.Domain.Enums;

namespace Transport.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public string PlateNumber { get; private set; }
        public int Capacity { get; private set; }
        public VehicleStatus Status { get; private set; }

        private Vehicle() { } // EF Core

        public Vehicle(string plateNumber, int capacity)
        {
            SetPlateNumber(plateNumber);
            SetCapacity(capacity);
            Status = VehicleStatus.Active;
        }

        public void SetPlateNumber(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                throw new DomainException("El numero de placa es obligatorio");

            PlateNumber = plate;
        }

        public void SetCapacity(int capacity)
        {
            if (capacity <= 0)
                throw new DomainException("La capacidad debe ser mayor que 0");

            Capacity = capacity;
        }

        public void Activate()
        {
            Status = VehicleStatus.Active;
        }

        public void Deactivate()
        {
            if (Status == VehicleStatus.Inactive)
                throw new DomainException("El vehiculo ya esta inhactivo");

            Status = VehicleStatus.Inactive;
        }
    }
}