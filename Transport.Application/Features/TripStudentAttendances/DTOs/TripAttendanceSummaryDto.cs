namespace Transport.Application.Features.TripStudentAttendances.DTOs
{
    public class TripAttendanceSummaryDto
    {
        public Guid TripId { get; set; }
        public Guid RouteAssignmentId { get; set; }
        public Guid? RouteId { get; set; }
        public string? RouteName { get; set; }
        public DateOnly? OperationDate { get; set; }
        public DateTime? ScheduledDepartureTime { get; set; }
        public DateTime? StartTime { get; set; }
        public int ExpectedCount { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int ExceptionalCount { get; set; }
        public IReadOnlyCollection<TripStudentAttendanceDto> ExpectedStudents { get; set; } = Array.Empty<TripStudentAttendanceDto>();
        public IReadOnlyCollection<TripStudentAttendanceDto> PresentStudents { get; set; } = Array.Empty<TripStudentAttendanceDto>();
        public IReadOnlyCollection<TripStudentAttendanceDto> AbsentStudents { get; set; } = Array.Empty<TripStudentAttendanceDto>();
        public IReadOnlyCollection<TripStudentAttendanceDto> ExceptionalStudents { get; set; } = Array.Empty<TripStudentAttendanceDto>();
        public IReadOnlyCollection<TripStudentAttendanceDto> AllStudents { get; set; } = Array.Empty<TripStudentAttendanceDto>();
    }
}
