using System.Text.Json.Serialization;
using Transport.Domain.Enums;

namespace Transport.Application.Features.TripRouteDeviations.DTOs
{
    public class TripRouteDeviationDto
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Guid ReportedByUserId { get; set; }
        public string? ReportedByName { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RouteDeviationReasonType ReasonType { get; set; }
        public string ReasonTypeLabel { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime ReportedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
