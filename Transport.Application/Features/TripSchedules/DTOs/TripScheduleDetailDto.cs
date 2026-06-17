using Transport.Domain.Enums;

namespace Transport.Application.Features.TripSchedules.DTOs
{
    public class TripScheduleDetailDto
    {
        public Guid Id { get; set; }
        public Guid RouteAssignmentId { get; set; }
        public TripDirection Direction { get; set; }
        public string DirectionLabel { get; set; } = string.Empty;
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public bool IsActive { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidTo { get; set; }
        public string DaysOfWeek { get; set; } = string.Empty;
        public string? RouteName { get; set; }
        public string? DriverName { get; set; }
        public string? VehiclePlate { get; set; }
    }
}
