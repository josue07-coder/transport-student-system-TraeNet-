using Transport.Domain.Enums;

namespace Transport.Domain.Entities
{
    public class Driver
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public DriverLicense DriverLicense { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public String Address { get; set; }
        public string PhoneNumber { get; set; }

        public Gender Gender { get; set; }

        public ICollection<RouteAssignment> RouteAssignments { get; set; } = new List<RouteAssignment>();
        public ICollection<VehicleAssignment> vehicleAssignments { get; set; } = new List<VehicleAssignment>();
    }
}
