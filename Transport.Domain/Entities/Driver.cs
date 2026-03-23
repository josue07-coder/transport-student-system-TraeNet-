using Transport.Domain.Enums;
using Transport.Domain.ValueObjects;
using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class Driver: BaseEntity
    {
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public LicenseNumber LicenseNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        public string? PhotoUrl { get; set; }

        public Gender Gender { get; set; }

        public ICollection<RouteAssignment> RouteAssignments { get; set; } = new List<RouteAssignment>();
        public ICollection<VehicleAssignment> VehicleAssignments { get; set; } = new List<VehicleAssignment>();
    }
}
