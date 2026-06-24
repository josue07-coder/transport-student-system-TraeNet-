using Transport.Domain.Enums;

namespace Transport.Application.Features.TripStudentAttendances.DTOs
{
    public class StudentAttendanceHistoryDto
    {
        public Guid TripId { get; set; }
        public Guid RouteAssignmentId { get; set; }
        public string? RouteName { get; set; }
        public DateOnly? OperationDate { get; set; }
        public DateTime? TripStartTime { get; set; }
        public TripAttendanceStatus Status { get; set; }
        public DateTime? BoardedAt { get; set; }
        public DateTime? DroppedOffAt { get; set; }
        public bool IsExpectedPassenger { get; set; }
        public string? ExceptionReason { get; set; }
        public AttendanceSource AttendanceSource { get; set; }
    }
}
