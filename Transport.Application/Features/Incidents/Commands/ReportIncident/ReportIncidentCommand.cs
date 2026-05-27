using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Incidents.Commands.ReportIncident
{
    public class ReportIncidentCommand : IRequest<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IncidentType Type { get; set; }
        public IncidentSeverity Severity { get; set; }
        public Guid? TripId { get; set; }
        public Guid? RouteAssignmentId { get; set; }
        public Guid? VehicleId { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? TransportAssistantId { get; set; }
    }
}
