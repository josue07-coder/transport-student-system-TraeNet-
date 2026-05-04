using Transport.Domain.Enums;
using Transport.Domain.ValueObjects;
using Transport.Domain.Exceptions;
using Transport.Domain.Common;

namespace Transport.Domain.Entities
{
    public class Route : BaseEntity
    {
        public string Name { get; private set; }
        public Guid SchoolId { get; private set; }
        public TimeRange OperatingHours { get; private set; }
        public RouteStatus Status { get; private set; }

        private readonly List<RouteStop> _stops = new();
        public IReadOnlyCollection<RouteStop> Stops => _stops.AsReadOnly();

        private readonly List<RouteAssignment> _assignments = new();
        public IReadOnlyCollection<RouteAssignment> Assignments => _assignments.AsReadOnly();

        private Route() { } // EF Core

        public Route(string name, Guid schoolId, TimeRange operatingHours)
        {
            SetName(name);

            if (schoolId == Guid.Empty)
                throw new DomainException("La escuela es obligatoria.");

            SchoolId = schoolId;
            OperatingHours = operatingHours ?? throw new DomainException("El horario es obligatorio");
            Status = RouteStatus.Inactive;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("El nombre de ruta es obligatorio");

            Name = name;
        }

        public void UpdateOperatingHours(TimeRange operatingHours)
        {
            if (Status == RouteStatus.Active)
                throw new DomainException("No se puede cambiar el horario mientras la ruta está activa");

            OperatingHours = operatingHours ?? throw new DomainException("El horario es obligatorio");
        }

        public void AddStop(RouteStop stop)
        {
            if (stop == null)
                throw new DomainException("La parada es obligatoria");

            if (_stops.Any(s => s.Equals(stop)))
                throw new DomainException("La parada ya existe en la ruta");

            _stops.Add(stop);
        }

        public void RemoveStop(Guid stopId)
        {
            var stop = _stops.FirstOrDefault(s => s.Id == stopId);

            if (stop == null)
                throw new DomainException("Parada no encontrada");

            if (Status == RouteStatus.Active)
                throw new DomainException("No se pueden eliminar paradas de una ruta activa");

            _stops.Remove(stop);
        }

        public void Assign(Guid driverId, Guid vehicleId, int capacity)
        {
            if (Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para asignar");

            if (driverId == Guid.Empty || vehicleId == Guid.Empty)
                throw new DomainException("El conductor y el vehiculo son obligatorios");

            _assignments.Add(new RouteAssignment(Id, driverId, vehicleId, capacity));
        }

        public void Activate()
        {
            if (!_stops.Any())
                throw new DomainException("La ruta debe tener al menos una parada");

            Status = RouteStatus.Active;
        }

        public void Deactivate()
        {
            Status = RouteStatus.Inactive;
        }
    }
}