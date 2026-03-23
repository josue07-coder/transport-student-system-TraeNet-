using Transport.Domain.Enums;
using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class Vehicle: BaseEntity
    {
       
        public string PlateNumber { get; set; }
        public string Brand { get; set; }
        public String Model { get; set; }
        public int Capacity { get; set; }
        public string? PhotoUrl { get; set; }

        public VehicleStatus Status { get; set; }

        public ICollection<RouteAssignment> RouteAssignments { get; set; } = new List<RouteAssignment>();
        public ICollection<VehicleAssignment> vehicleAssignments { get; set; } = new List<VehicleAssignment>();

    }
}
