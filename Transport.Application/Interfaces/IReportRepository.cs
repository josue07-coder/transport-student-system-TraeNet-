using Transport.Application.Features.Reports.DTOs;

namespace Transport.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<DashboardReportDto> GetDashboardAsync(CancellationToken cancellationToken = default);
        Task<List<TripReportDto>> GetTripsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<StudentsByRouteReportDto> GetStudentsByRouteAsync(Guid routeId, CancellationToken cancellationToken = default);
        Task<List<IncidentReportDto>> GetIncidentsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<DriverPerformanceReportDto>> GetDriversPerformanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<VehicleUsageReportDto>> GetVehiclesUsageAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<AuditSummaryReportDto>> GetAuditSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<LowPresenceStudentReportDto>> GetLowPresenceReportAsync(DateTime startDate, DateTime endDate, decimal maximumPresencePercentage, CancellationToken cancellationToken = default);
    }
}
