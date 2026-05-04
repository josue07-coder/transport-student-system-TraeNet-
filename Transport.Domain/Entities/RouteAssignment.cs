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
                throw new DomainException("La ruta es obligatoria");

            if (vehicleId == Guid.Empty)
                throw new DomainException("El vehiculo es obligatorio");

            if (driverId == Guid.Empty)
                throw new DomainException("El conductor es obligatorio");

            if (capacity <= 0)
                throw new DomainException("La capacidad del vehículo debe ser mayor que 0");

            RouteId = routeId;
            VehicleId = vehicleId;
            DriverId = driverId;
            VehicleCapacity = capacity;
        }

        public void AssignStudent(Guid studentId)
        {
            if (studentId == Guid.Empty)
                throw new DomainException("El estudiante es obligatorio");

            if (_students.Count >= VehicleCapacity)
                throw new DomainException("Capacidad del vehiculo excedida");

            if (_students.Any(s => s.StudentId == studentId))
                throw new DomainException("El estudiante ya esta asignado");

            _students.Add(new StudentRouteAssignment(studentId, Id));
        }

        public void RemoveStudent(Guid studentId)
        {
            var student = _students.FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                throw new DomainException("El estudiante no se encuentra en esta ruta");

            _students.Remove(student);
        }

        public Trip StartTrip()
        {
            if (!_students.Any())
                throw new DomainException("No se puede iniciar el viaje sin estdiantes");

            if (_trips.Any(t => t.IsActive))
                throw new DomainException("Ya hay un viaje activo");

            var trip = new Trip(Id);
            _trips.Add(trip);

            return trip;
        }

        public void EndTrip(Guid tripId)
        {
            var trip = _trips.FirstOrDefault(t => t.Id == tripId);

            if (trip == null)
                throw new DomainException("Viaje no encontrado");

            trip.End();
        }
    }
}