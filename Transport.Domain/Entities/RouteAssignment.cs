using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class RouteAssignment : BaseEntity
    {
        public Guid RouteId { get; private set; }
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public int VehicleCapacity { get; private set; }

        private readonly List<StudentRouteAssignment> _students = new();
        public IReadOnlyCollection<StudentRouteAssignment> Students => _students.AsReadOnly();

        private readonly List<Trip> _trips = new();
        public IReadOnlyCollection<Trip> Trips => _trips.AsReadOnly();

        private RouteAssignment() { } // EF Core

        public RouteAssignment(Guid routeId, Guid vehicleId, Guid driverId, int capacity)
        {
            if (routeId == Guid.Empty)
                throw new DomainException("Route is required");

            if (vehicleId == Guid.Empty)
                throw new DomainException("Vehicle is required");

            if (driverId == Guid.Empty)
                throw new DomainException("Driver is required");

            if (capacity <= 0)
                throw new DomainException("Vehicle capacity must be greater than zero");

            RouteId = routeId;
            VehicleId = vehicleId;
            DriverId = driverId;
            VehicleCapacity = capacity;
        }

        public void AssignStudent(Guid studentId)
        {
            if (studentId == Guid.Empty)
                throw new DomainException("Student is required");

            if (_students.Count >= VehicleCapacity)
                throw new DomainException("Vehicle capacity exceeded");

            if (_students.Any(s => s.StudentId == studentId))
                throw new DomainException("Student already assigned");

            _students.Add(new StudentRouteAssignment(studentId, Id));
        }

        public void RemoveStudent(Guid studentId)
        {
            var student = _students.FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                throw new DomainException("Student not found in this route");

            _students.Remove(student);
        }

        public Trip StartTrip()
        {
            if (!_students.Any())
                throw new DomainException("Cannot start trip without students");

            if (_trips.Any(t => t.IsActive))
                throw new DomainException("There is already an active trip");

            var trip = new Trip(Id);
            _trips.Add(trip);

            return trip;
        }

        public void EndTrip(Guid tripId)
        {
            var trip = _trips.FirstOrDefault(t => t.Id == tripId);

            if (trip == null)
                throw new DomainException("Trip not found");

            trip.End();
        }
    }
}