using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class VehicleLocation : BaseEntity
    {
        public Guid TripId { get; private set; }
        public Trip Trip { get; private set; } = null!;

        public Guid VehicleId { get; private set; }
        public Vehicle Vehicle { get; private set; } = null!;

        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public decimal? Speed { get; private set; }
        public decimal? Heading { get; private set; }
        public DateTime RecordedAt { get; private set; }

        public Guid? ReportedByUserId { get; private set; }
        public User? ReportedByUser { get; private set; }

        private VehicleLocation() { } // EF

        public VehicleLocation(
            Guid tripId,
            Guid vehicleId,
            decimal latitude,
            decimal longitude,
            decimal? speed = null,
            decimal? heading = null,
            Guid? reportedByUserId = null)
        {
            if (tripId == Guid.Empty)
                throw new DomainException("Trip is required");

            if (vehicleId == Guid.Empty)
                throw new DomainException("Vehicle is required");

            ValidateCoordinates(latitude, longitude);

            if (speed.HasValue && speed.Value < 0)
                throw new DomainException("Speed cannot be negative");

            if (heading.HasValue && (heading.Value < 0 || heading.Value > 360))
                throw new DomainException("Heading must be between 0 and 360");

            TripId = tripId;
            VehicleId = vehicleId;
            Latitude = latitude;
            Longitude = longitude;
            Speed = speed;
            Heading = heading;
            ReportedByUserId = reportedByUserId;
            RecordedAt = DateTime.UtcNow;
        }

        private static void ValidateCoordinates(decimal latitude, decimal longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new DomainException("Latitude must be between -90 and 90");

            if (longitude < -180 || longitude > 180)
                throw new DomainException("Longitude must be between -180 and 180");
        }
    }
}
