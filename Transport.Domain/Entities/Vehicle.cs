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
                throw new DomainException("Plate number is required");

            PlateNumber = plate;
        }

        public void SetCapacity(int capacity)
        {
            if (capacity <= 0)
                throw new DomainException("Capacity must be greater than 0");

            Capacity = capacity;
        }

        public void Activate()
        {
            Status = VehicleStatus.Active;
        }

        public void Deactivate()
        {
            if (Status == VehicleStatus.Inactive)
                throw new DomainException("Vehicle is already inactive");

            Status = VehicleStatus.Inactive;
        }
    }
}