using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class RouteAssignment: BaseEntity
    {

        public int RouteId { get; set; }
        public Route Route { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public int DriverId { get; set; }
        public Driver Driver { get; set; }

        public DateTime StartDate { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();

        public ICollection<StudentRouteAssignment> StudentRouteAssignments { get; set; } = new List<StudentRouteAssignment>();
    }

}

