namespace Transport.Application.Features.TripStudentAttendances.DTOs
{
    public class AttendanceStudentSearchResultDto
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public Guid GuardianId { get; set; }
        public string GuardianName { get; set; } = string.Empty;
        public string? GuardianDocumentNumber { get; set; }
        public string? SchoolName { get; set; }
        public bool IsAssignedToRoute { get; set; }
        public bool AlreadyRegisteredInTrip { get; set; }
    }
}
