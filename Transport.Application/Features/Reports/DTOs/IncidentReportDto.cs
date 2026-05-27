using Transport.Domain.Enums;

namespace Transport.Application.Features.Reports.DTOs
{
    public class IncidentReportDto
    {
        public Guid IncidentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public IncidentType Type { get; set; }
        public IncidentSeverity Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public string? ReportedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
