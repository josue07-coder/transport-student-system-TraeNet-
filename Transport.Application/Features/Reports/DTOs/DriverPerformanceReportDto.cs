namespace Transport.Application.Features.Reports.DTOs
{
    public class DriverPerformanceReportDto
    {
        public Guid DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public int TotalTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }
        public int OpenIncidents { get; set; }
        public int CriticalIncidents { get; set; }
    }
}
