using Transport.Domain.Entities;

namespace Transport.Application.Features.Incidents.DTOs
{
    public static class IncidentMappings
    {
        public static IncidentResponseDto ToResponseDto(this Incident incident)
        {
            return new IncidentResponseDto
            {
                Id = incident.Id,
                Title = incident.Title,
                Type = incident.Type,
                Severity = incident.Severity,
                Status = incident.Status,
                TripId = incident.TripId,
                RouteAssignmentId = incident.RouteAssignmentId,
                ReportedByUserId = incident.ReportedByUserId,
                AssignedToUserId = incident.AssignedToUserId,
                CreatedAt = incident.CreatedAt,
                ResolvedAt = incident.ResolvedAt
            };
        }

        public static IncidentDetailDto ToDetailDto(this Incident incident)
        {
            return new IncidentDetailDto
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                Type = incident.Type,
                Severity = incident.Severity,
                Status = incident.Status,
                TripId = incident.TripId,
                RouteAssignmentId = incident.RouteAssignmentId,
                VehicleId = incident.VehicleId,
                DriverId = incident.DriverId,
                TransportAssistantId = incident.TransportAssistantId,
                ReportedByUserId = incident.ReportedByUserId,
                AssignedToUserId = incident.AssignedToUserId,
                ResolvedByUserId = incident.ResolvedByUserId,
                CreatedAt = incident.CreatedAt,
                ResolvedAt = incident.ResolvedAt,
                ClosedAt = incident.ClosedAt,
                Comments = incident.Comments
                    .OrderBy(comment => comment.CreatedAt)
                    .Select(comment => new IncidentCommentDto
                    {
                        Id = comment.Id,
                        UserId = comment.UserId,
                        Comment = comment.Comment,
                        CreatedAt = comment.CreatedAt
                    })
                    .ToList()
            };
        }
    }
}
