namespace Transport.Application.Features.Reports.DTOs
{
    public class VehicleUsageReportDto
    {
        public Guid VehicleId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int TotalTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }
        public int IncidentsCount { get; set; }
    }
}
