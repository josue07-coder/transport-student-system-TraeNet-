using Microsoft.EntityFrameworkCore;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardReportDto> GetDashboardAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return new DashboardReportDto
            {
                TotalStudents = await _context.Students.AsNoTracking().CountAsync(cancellationToken),
                TotalGuardians = await _context.Guardians.AsNoTracking().CountAsync(cancellationToken),
                TotalSchools = await _context.Schools.AsNoTracking().CountAsync(cancellationToken),
                TotalVehicles = await _context.Vehicles.AsNoTracking().CountAsync(cancellationToken),
                TotalDrivers = await _context.Drivers.AsNoTracking().CountAsync(cancellationToken),
                TotalTransportAssistants = await _context.TransportAssistants.AsNoTracking().CountAsync(cancellationToken),
                TotalRoutes = await _context.Routes.AsNoTracking().CountAsync(cancellationToken),
                TotalActiveRoutes = await _context.Routes.AsNoTracking().CountAsync(x => x.Status == RouteStatus.Active, cancellationToken),
                TotalTrips = await _context.Trips.AsNoTracking().CountAsync(cancellationToken),
                ActiveTrips = await _context.Trips.AsNoTracking().CountAsync(x => x.Status == TripStatus.InProgress, cancellationToken),
                CompletedTrips = await _context.Trips.AsNoTracking().CountAsync(x => x.Status == TripStatus.Completed, cancellationToken),
                CancelledTrips = await _context.Trips.AsNoTracking().CountAsync(x => x.Status == TripStatus.Cancelled, cancellationToken),
                OpenIncidents = await _context.Incidents.AsNoTracking().CountAsync(x => x.Status == IncidentStatus.Open || x.Status == IncidentStatus.InProgress, cancellationToken),
                CriticalIncidents = await _context.Incidents.AsNoTracking().CountAsync(x => x.Severity == IncidentSeverity.Critical && x.Status != IncidentStatus.Closed && x.Status != IncidentStatus.Cancelled, cancellationToken),
                UnreadNotifications = await _context.Notifications.AsNoTracking().CountAsync(x => !x.IsRead, cancellationToken),
                TodayTrips = await _context.Trips.AsNoTracking().CountAsync(x => x.StartTime >= today && x.StartTime < tomorrow, cancellationToken)
            };
        }

        public Task<List<TripReportDto>> GetTripsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var (start, endExclusive) = NormalizeRange(startDate, endDate);

            return _context.Trips
                .AsNoTracking()
                .Where(x => (x.StartTime ?? x.CreatedAt) >= start && (x.StartTime ?? x.CreatedAt) < endExclusive)
                .OrderByDescending(x => x.StartTime ?? x.CreatedAt)
                .Select(x => new TripReportDto
                {
                    TripId = x.Id,
                    RouteName = x.RouteAssignment.Route.Name,
                    DriverName = x.RouteAssignment.Driver.FirstName + " " + x.RouteAssignment.Driver.LastName,
                    VehiclePlate = x.RouteAssignment.Vehicle.PlateNumber,
                    Status = x.Status,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    DurationMinutes = x.StartTime.HasValue && x.EndTime.HasValue
                        ? EF.Functions.DateDiffMinute(x.StartTime.Value, x.EndTime.Value)
                        : null,
                    StudentsCount = x.RouteAssignment.Students.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<StudentsByRouteReportDto> GetStudentsByRouteAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            var route = await _context.Routes
                .AsNoTracking()
                .Where(x => x.Id == routeId)
                .Select(x => new { x.Id, x.Name })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new DomainException("Ruta no encontrada");

            var students = await _context.StudentRouteAssignments
                .AsNoTracking()
                .Where(x => x.RouteAssignment.RouteId == routeId)
                .OrderBy(x => x.Student.LastName)
                .ThenBy(x => x.Student.FirstName)
                .Select(x => new StudentRouteReportItemDto
                {
                    StudentId = x.StudentId,
                    FullName = x.Student.FirstName + " " + x.Student.LastName,
                    StudentCode = x.Student.StudentCode.Value,
                    GuardianName = x.Student.Guardian.FirstName + " " + x.Student.Guardian.LastName,
                    GuardianPhone = x.Student.Guardian.Phone,
                    GradeName = x.Student.Grade.Name,
                    SchoolName = x.Student.School == null ? string.Empty : x.Student.School.Name
                })
                .ToListAsync(cancellationToken);

            return new StudentsByRouteReportDto
            {
                RouteId = route.Id,
                RouteName = route.Name,
                Students = students
            };
        }

        public Task<List<IncidentReportDto>> GetIncidentsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var (start, endExclusive) = NormalizeRange(startDate, endDate);

            return _context.Incidents
                .AsNoTracking()
                .Where(x => x.CreatedAt >= start && x.CreatedAt < endExclusive)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new IncidentReportDto
                {
                    IncidentId = x.Id,
                    Title = x.Title,
                    Type = x.Type,
                    Severity = x.Severity,
                    Status = x.Status,
                    ReportedBy = x.ReportedByUser.Name,
                    CreatedAt = x.CreatedAt,
                    ResolvedAt = x.ResolvedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DriverPerformanceReportDto>> GetDriversPerformanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var (start, endExclusive) = NormalizeRange(startDate, endDate);

            var tripStats = await _context.Trips
                .AsNoTracking()
                .Where(x => (x.StartTime ?? x.CreatedAt) >= start && (x.StartTime ?? x.CreatedAt) < endExclusive)
                .GroupBy(x => new
                {
                    x.RouteAssignment.DriverId,
                    x.RouteAssignment.Driver.FirstName,
                    x.RouteAssignment.Driver.LastName
                })
                .Select(x => new DriverPerformanceReportDto
                {
                    DriverId = x.Key.DriverId,
                    DriverName = x.Key.FirstName + " " + x.Key.LastName,
                    TotalTrips = x.Count(),
                    CompletedTrips = x.Count(t => t.Status == TripStatus.Completed),
                    CancelledTrips = x.Count(t => t.Status == TripStatus.Cancelled)
                })
                .ToListAsync(cancellationToken);

            var incidentStats = await _context.Incidents
                .AsNoTracking()
                .Where(x => x.DriverId.HasValue && x.CreatedAt >= start && x.CreatedAt < endExclusive)
                .GroupBy(x => x.DriverId!.Value)
                .Select(x => new
                {
                    DriverId = x.Key,
                    OpenIncidents = x.Count(i => i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress),
                    CriticalIncidents = x.Count(i => i.Severity == IncidentSeverity.Critical)
                })
                .ToListAsync(cancellationToken);

            foreach (var report in tripStats)
            {
                var incidents = incidentStats.FirstOrDefault(x => x.DriverId == report.DriverId);
                if (incidents == null)
                    continue;

                report.OpenIncidents = incidents.OpenIncidents;
                report.CriticalIncidents = incidents.CriticalIncidents;
            }

            return tripStats.OrderBy(x => x.DriverName).ToList();
        }

        public async Task<List<VehicleUsageReportDto>> GetVehiclesUsageAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var (start, endExclusive) = NormalizeRange(startDate, endDate);

            var tripStats = await _context.Trips
                .AsNoTracking()
                .Where(x => (x.StartTime ?? x.CreatedAt) >= start && (x.StartTime ?? x.CreatedAt) < endExclusive)
                .GroupBy(x => new
                {
                    x.RouteAssignment.VehicleId,
                    x.RouteAssignment.Vehicle.PlateNumber,
                    x.RouteAssignment.Vehicle.Capacity
                })
                .Select(x => new VehicleUsageReportDto
                {
                    VehicleId = x.Key.VehicleId,
                    PlateNumber = x.Key.PlateNumber,
                    Capacity = x.Key.Capacity,
                    TotalTrips = x.Count(),
                    CompletedTrips = x.Count(t => t.Status == TripStatus.Completed),
                    CancelledTrips = x.Count(t => t.Status == TripStatus.Cancelled)
                })
                .ToListAsync(cancellationToken);

            var incidentStats = await _context.Incidents
                .AsNoTracking()
                .Where(x => x.VehicleId.HasValue && x.CreatedAt >= start && x.CreatedAt < endExclusive)
                .GroupBy(x => x.VehicleId!.Value)
                .Select(x => new { VehicleId = x.Key, IncidentsCount = x.Count() })
                .ToListAsync(cancellationToken);

            foreach (var report in tripStats)
            {
                report.IncidentsCount = incidentStats.FirstOrDefault(x => x.VehicleId == report.VehicleId)?.IncidentsCount ?? 0;
            }

            return tripStats.OrderBy(x => x.PlateNumber).ToList();
        }

        public Task<List<AuditSummaryReportDto>> GetAuditSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var (start, endExclusive) = NormalizeRange(startDate, endDate);

            return _context.AuditLogs
                .AsNoTracking()
                .Where(x => x.CreatedAt >= start && x.CreatedAt < endExclusive)
                .GroupBy(x => x.Action)
                .Select(x => new AuditSummaryReportDto
                {
                    Action = x.Key,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Action)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<LowPresenceStudentReportDto>> GetLowPresenceReportAsync(
            DateTime startDate,
            DateTime endDate,
            decimal maximumPresencePercentage,
            CancellationToken cancellationToken = default)
        {
            var (start, endExclusive) = NormalizeRange(startDate, endDate);

            var attendanceStats = await _context.TripStudentAttendances
                .AsNoTracking()
                .Where(x => x.IsExpectedPassenger
                    && x.CreatedAt >= start
                    && x.CreatedAt < endExclusive)
                .GroupBy(x => new
                {
                    x.StudentId,
                    x.StudentNameSnapshot,
                    x.StudentCodeSnapshot,
                    x.GuardianIdSnapshot,
                    x.GuardianNameSnapshot
                })
                .Select(x => new
                {
                    x.Key.StudentId,
                    StudentName = x.Key.StudentNameSnapshot,
                    StudentCode = x.Key.StudentCodeSnapshot,
                    GuardianId = x.Key.GuardianIdSnapshot,
                    GuardianName = x.Key.GuardianNameSnapshot,
                    ExpectedTrips = x.Count(),
                    PresentTrips = x.Count(a => a.Status == TripAttendanceStatus.Boarded || a.Status == TripAttendanceStatus.DroppedOff),
                    AbsentTrips = x.Count(a => a.Status == TripAttendanceStatus.Absent),
                    LastAttendanceAt = x.Max(a => a.MarkedAt ?? a.BoardedAt ?? a.DroppedOffAt ?? a.CreatedAt)
                })
                .ToListAsync(cancellationToken);

            var exceptionalStats = await _context.TripStudentAttendances
                .AsNoTracking()
                .Where(x => !x.IsExpectedPassenger
                    && x.CreatedAt >= start
                    && x.CreatedAt < endExclusive)
                .GroupBy(x => x.StudentId)
                .Select(x => new { StudentId = x.Key, Count = x.Count() })
                .ToListAsync(cancellationToken);

            return attendanceStats
                .Select(x =>
                {
                    var presencePercentage = x.ExpectedTrips == 0
                        ? 0
                        : Math.Round((decimal)x.PresentTrips * 100 / x.ExpectedTrips, 2);

                    return new LowPresenceStudentReportDto
                    {
                        StudentId = x.StudentId,
                        StudentName = x.StudentName,
                        StudentCode = x.StudentCode,
                        GuardianId = x.GuardianId,
                        GuardianName = x.GuardianName,
                        ExpectedTrips = x.ExpectedTrips,
                        PresentTrips = x.PresentTrips,
                        AbsentTrips = x.AbsentTrips,
                        ExceptionalBoardings = exceptionalStats.FirstOrDefault(e => e.StudentId == x.StudentId)?.Count ?? 0,
                        PresencePercentage = presencePercentage,
                        LastAttendanceAt = x.LastAttendanceAt
                    };
                })
                .Where(x => x.PresencePercentage <= maximumPresencePercentage)
                .OrderBy(x => x.PresencePercentage)
                .ThenByDescending(x => x.AbsentTrips)
                .ThenBy(x => x.StudentName)
                .ToList();
        }

        private static (DateTime Start, DateTime EndExclusive) NormalizeRange(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var endExclusive = endDate.Date.AddDays(1);
            return (start, endExclusive);
        }
    }
}
