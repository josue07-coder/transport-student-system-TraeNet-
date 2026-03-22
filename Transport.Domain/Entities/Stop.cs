
namespace Transport.Domain.Entities
{
    public class Stop
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Guid SectorId { get; set; }
        public Sector Sector { get; set; }

        public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
        public ICollection<StudentRouteAssignment> StudentRouteAssignments { get; set; } = new List<StudentRouteAssignment>();
    }
}

