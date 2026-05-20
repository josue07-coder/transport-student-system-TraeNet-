namespace Transport.Application.Features.Stops.DTOs
{
    public class StopDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SectorId { get; set; }
        public string? SectorName { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
