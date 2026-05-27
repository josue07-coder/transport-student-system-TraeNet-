using Transport.Domain.Enums;

namespace Transport.Application.Features.Reports.DTOs
{
    public class TripReportDto
    {
        public Guid TripId { get; set; }
        public string? RouteName { get; set; }
        public string? DriverName { get; set; }
        public string? VehiclePlate { get; set; }
        public TripStatus Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? DurationMinutes { get; set; }
        public int StudentsCount { get; set; }
    }
}
