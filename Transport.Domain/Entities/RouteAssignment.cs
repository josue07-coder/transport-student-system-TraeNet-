

namespace Transport.Domain.Entities
{
    public class RouteAssignment
    {
        public Guid Id { get; set; }

        public Guid RouteId { get; set; }

        public Guid VehicleId { get; set; }

        public Guid DriverId { get; set; }

        public DateTime StartDate { get; set; }

        public Route Route { get; set; }

        public Vehicle Vehicle { get; set; }

        public Driver Driver { get; set; }
    }
}
