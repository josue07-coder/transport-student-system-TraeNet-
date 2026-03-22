

namespace Transport.Domain.Entities
{
    public class VehicleAssignment
    {
        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public Guid DriverId { get; set; }
        public User Driver { get; set; }

        public DateTime AssignmentDate { get; set; }
    }
}
