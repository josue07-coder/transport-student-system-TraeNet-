using Transport.Domain.Common;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Entities
{
    public class Stop : BaseEntity
    {
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public Coordinates Coordinates { get; private set; }
        public Guid SectorId { get; private set; }

        private Stop() { } // EF Core

        public Stop(string name, Address address, Coordinates coordinates, Guid sectorId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("EL nombre de la parada es obligatorio");

            if (sectorId == Guid.Empty)
                throw new DomainException("El sector es obligatorio");

            Name = name;
            Address = address ?? throw new DomainException("La direccion es obligatorio");
            Coordinates = coordinates ?? throw new DomainException("Las coordenadas son obligatorias");
            SectorId = sectorId;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Stop name is required");

            Name = name;
        }

        public void UpdateLocation(Address address, Coordinates coordinates)
        {
            Address = address ?? throw new DomainException("Address is required");
            Coordinates = coordinates ?? throw new DomainException("Coordinates are required");
        }

        public void ChangeSector(Guid sectorId)
        {
            if (sectorId == Guid.Empty)
                throw new DomainException("Sector is required");

            SectorId = sectorId;
        }
    }
}