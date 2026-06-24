using Transport.Domain.Enums;

namespace Transport.Application.Features.Trips.DTOs
{
    public class TripResponseDto
    {
        public Guid Id { get; set; }
        public Guid RouteAssignmentId { get; set; }
        public Guid? TripScheduleId { get; set; }
        public TripDirection? Direction { get; set; }
        public DateOnly? OperationDate { get; set; }
        public DateTime? ScheduledDepartureTime { get; set; }
        public DateTime? ScheduledArrivalTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TripStatus Status { get; set; }
        public string? CancellationReason { get; set; }
        public string? NonOperationReason { get; set; }
        public string? NonOperationNotes { get; set; }
        public int DelayMinutes { get; set; }
        public bool IsLate { get; set; }
        public bool StartedEarly { get; set; }
        public string? EarlyStartReason { get; set; }
        public TripPunctualityStatus PunctualityStatus { get; set; }
    }
}
