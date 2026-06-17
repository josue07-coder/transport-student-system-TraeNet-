using Transport.Domain.Enums;

namespace Transport.Application.Features.TripStudentAttendances.DTOs
{
    public class TripStudentAttendanceDto
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public Guid? GuardianId { get; set; }
        public string? GuardianName { get; set; }
        public TripAttendanceStatus Status { get; set; }
        public DateTime? BoardedAt { get; set; }
        public DateTime? DroppedOffAt { get; set; }
        public string? Notes { get; set; }
    }
}
