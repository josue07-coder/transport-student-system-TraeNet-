namespace Transport.Application.Features.Reports.DTOs
{
    public class StudentRouteReportItemDto
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string GuardianPhone { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
    }
}
