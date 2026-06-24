namespace Transport.Application.Features.Reports.DTOs
{
    public class LowPresenceStudentReportDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public Guid? GuardianId { get; set; }
        public string? GuardianName { get; set; }
        public int ExpectedTrips { get; set; }
        public int PresentTrips { get; set; }
        public int AbsentTrips { get; set; }
        public int ExceptionalBoardings { get; set; }
        public decimal PresencePercentage { get; set; }
        public DateTime? LastAttendanceAt { get; set; }
    }
}
