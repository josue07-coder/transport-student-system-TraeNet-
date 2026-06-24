using Transport.Domain.Enums;

namespace Transport.Application.Features.TripStudentAttendances.DTOs
{
    public class TripStudentAttendanceDto
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public string StudentNameSnapshot { get; set; } = string.Empty;
        public string StudentCodeSnapshot { get; set; } = string.Empty;
        public Guid? GuardianId { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianNameSnapshot { get; set; }
        public string? SchoolNameSnapshot { get; set; }
        public TripAttendanceStatus Status { get; set; }
        public DateTime? BoardedAt { get; set; }
        public DateTime? DroppedOffAt { get; set; }
        public string? Notes { get; set; }
        public bool IsExpectedPassenger { get; set; }
        public string? ExceptionReason { get; set; }
        public Guid? RegisteredByUserId { get; set; }
        public string? RegisteredByName { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public Guid? MarkedByUserId { get; set; }
        public string? MarkedByName { get; set; }
        public DateTime? MarkedAt { get; set; }
        public DateTime? AbsenceNotifiedAt { get; set; }
        public DateTime? BoardedNotificationSentAt { get; set; }
        public AttendanceSource AttendanceSource { get; set; }
    }
}
