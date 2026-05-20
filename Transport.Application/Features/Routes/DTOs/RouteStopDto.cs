namespace Transport.Application.Features.Routes.DTOs
{
    public class RouteStopDto
    {
        public Guid RouteStopId { get; set; }
        public Guid StopId { get; set; }
        public string StopName { get; set; } = string.Empty;
        public int StopOrder { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
