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
                throw new DomainException("School is required");

            SchoolId = schoolId;
            OperatingHours = operatingHours ?? throw new DomainException("Operating hours are required");
            Status = RouteStatus.Inactive;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Route name is required");

            Name = name;
        }

        public void UpdateOperatingHours(TimeRange operatingHours)
        {
            if (Status == RouteStatus.Active)
                throw new DomainException("Cannot change schedule while route is active");

            OperatingHours = operatingHours ?? throw new DomainException("Operating hours are required");
        }

        public void AddStop(RouteStop stop)
        {
            if (stop == null)
                throw new DomainException("Stop is required");

            if (_stops.Any(s => s.Equals(stop)))
                throw new DomainException("Stop already exists in route");

            _stops.Add(stop);
        }

        public void RemoveStop(Guid stopId)
        {
            var stop = _stops.FirstOrDefault(s => s.Id == stopId);

            if (stop == null)
                throw new DomainException("Stop not found");

            if (Status == RouteStatus.Active)
                throw new DomainException("Cannot remove stops from an active route");

            _stops.Remove(stop);
        }

        public void Assign(Guid driverId, Guid vehicleId)
        {
            if (Status != RouteStatus.Active)
                throw new DomainException("Route must be active to assign");

            if (driverId == Guid.Empty || vehicleId == Guid.Empty)
                throw new DomainException("Driver and Vehicle are required");

            _assignments.Add(new RouteAssignment(Id, driverId, vehicleId, capacity));
        }

        public void Activate()
        {
            if (!_stops.Any())
                throw new DomainException("Route must have at least one stop");

            Status = RouteStatus.Active;
        }

        public void Deactivate()
        {
            Status = RouteStatus.Inactive;
        }
    }
}