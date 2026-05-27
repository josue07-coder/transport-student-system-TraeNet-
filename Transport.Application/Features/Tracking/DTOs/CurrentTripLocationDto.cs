namespace Transport.Application.Features.Tracking.DTOs
{
    public class CurrentTripLocationDto
    {
        public Guid TripId { get; set; }
        public Guid VehicleId { get; set; }
        public string? RouteName { get; set; }
        public string? DriverName { get; set; }
        public string? TransportAssistantName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? Speed { get; set; }
        public decimal? Heading { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
