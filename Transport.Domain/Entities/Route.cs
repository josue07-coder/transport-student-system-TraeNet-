using Transport.Domain.Enums;

namespace Transport.Domain.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid SchoolId { get; set; }
        public School School { get; set; }

        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ReturnTime { get; set; }
        public RouteStatus Status { get; set; }

        public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
        public ICollection<RouteAssignment> RouteAssignments { get; set; } = new List<RouteAssignment>();

    }
}
