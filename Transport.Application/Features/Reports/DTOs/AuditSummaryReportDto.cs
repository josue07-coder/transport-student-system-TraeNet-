namespace Transport.Application.Features.Reports.DTOs
{
    public class AuditSummaryReportDto
    {
        public string Action { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
