using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Trip : BaseEntity
    {
        public Guid RouteAssignmentId { get; private set; }
        public RouteAssignment RouteAssignment { get; private set; } = null!;

        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        public TripStatus Status { get; private set; }

        public bool IsActive => Status == TripStatus.InProgress;

        private Trip() { } // EF Core

        public Trip(Guid routeAssignmentId)
        {
            if (routeAssignmentId == Guid.Empty)
                throw new DomainException("Route assignment is required");

            RouteAssignmentId = routeAssignmentId;
            Status = TripStatus.Pending;
        }

        public void Start()
        {
            if (Status != TripStatus.Pending)
                throw new DomainException("Trip already started or completed");

            StartTime = DateTime.UtcNow;
            Status = TripStatus.InProgress;
        }

        public void End()
        {
            if (Status != TripStatus.InProgress)
                throw new DomainException("Trip is not in progress");

            EndTime = DateTime.UtcNow;
            Status = TripStatus.Completed;
        }

        public void Cancel()
        {
            // Regla actual: el dominio permite cancelar viajes Pending e InProgress.
            if (Status == TripStatus.Completed)
                throw new DomainException("No se puede cancelar un viaje completado");

            if (Status == TripStatus.Cancelled)
                throw new DomainException("El viaje ya está cancelado");

            EndTime = DateTime.UtcNow;
            Status = TripStatus.Cancelled;
        }
    }
}
