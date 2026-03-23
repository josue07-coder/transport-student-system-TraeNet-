using Transport.Domain.ValueObjects;
using Transport.Shared.Common;
namespace Transport.Domain.Entities
{
    public class Stop: BaseEntity
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public Coordinates Coordinates { get; set; }

        public Guid SectorId { get; set; }
        public Sector Sector { get; set; }

        public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
        public ICollection<StudentRouteAssignment> StudentRouteAssignments { get; set; } = new List<StudentRouteAssignment>();
    }
}

