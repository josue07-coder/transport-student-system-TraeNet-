namespace Transport.Application.Features.RouteAssignments.DTOs
{
    public class RouteAssignmentDetailDto
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public Guid DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public Guid VehicleId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public Guid? TransportAssistantId { get; set; }
        public string? TransportAssistantName { get; set; }
        public int VehicleCapacity { get; set; }
        public int TripsCount { get; set; }
        public List<AssignedStudentDto> Students { get; set; } = new();
    }
}
