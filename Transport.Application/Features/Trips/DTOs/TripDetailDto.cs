using Transport.Domain.Enums;

namespace Transport.Application.Features.Trips.DTOs
{
    public class TripDetailDto
    {
        public Guid Id { get; set; }
        public Guid RouteAssignmentId { get; set; }
        public Guid? RouteId { get; set; }
        public string? RouteName { get; set; }
        public Guid? DriverId { get; set; }
        public string? DriverName { get; set; }
        public Guid? VehicleId { get; set; }
        public string? PlateNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TripStatus Status { get; set; }
    }
}
