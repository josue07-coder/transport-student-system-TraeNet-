using Transport.Domain.Enums;

namespace Transport.Application.Features.Incidents.DTOs
{
    public class IncidentResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public IncidentType Type { get; set; }
        public IncidentSeverity Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public Guid? TripId { get; set; }
        public Guid? RouteAssignmentId { get; set; }
        public Guid ReportedByUserId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
