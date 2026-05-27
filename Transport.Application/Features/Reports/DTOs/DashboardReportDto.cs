namespace Transport.Application.Features.Reports.DTOs
{
    public class DashboardReportDto
    {
        public int TotalStudents { get; set; }
        public int TotalGuardians { get; set; }
        public int TotalSchools { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalTransportAssistants { get; set; }
        public int TotalRoutes { get; set; }
        public int TotalActiveRoutes { get; set; }
        public int TotalTrips { get; set; }
        public int ActiveTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }
        public int OpenIncidents { get; set; }
        public int CriticalIncidents { get; set; }
        public int UnreadNotifications { get; set; }
        public int TodayTrips { get; set; }
    }
}
