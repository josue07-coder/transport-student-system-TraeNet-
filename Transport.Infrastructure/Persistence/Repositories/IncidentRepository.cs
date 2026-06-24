using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly AppDbContext _context;

        public IncidentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Incident incident)
        {
            await _context.Incidents.AddAsync(incident);
        }

        public async Task AddCommentAsync(IncidentComment comment)
        {
            await _context.IncidentComments.AddAsync(comment);
        }

        public async Task<Incident?> GetByIdAsync(Guid id)
        {
            return await IncidentQuery()
                .FirstOrDefaultAsync(incident => incident.Id == id);
        }

        public async Task<Incident?> GetByIdWithDetailsAsync(Guid id)
        {
            return await IncidentQuery()
                .Include(incident => incident.Comments)
                    .ThenInclude(comment => comment.User)
                .FirstOrDefaultAsync(incident => incident.Id == id);
        }

        public async Task<PaginatedResponse<Incident>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = IncidentQuery(asNoTracking: true);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Incident>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<Incident>> GetByStatusAsync(IncidentStatus status)
        {
            return await IncidentQuery(asNoTracking: true)
                .Where(incident => incident.Status == status)
                .ToListAsync();
        }

        public async Task<List<Incident>> GetBySeverityAsync(IncidentSeverity severity)
        {
            return await IncidentQuery(asNoTracking: true)
                .Where(incident => incident.Severity == severity)
                .ToListAsync();
        }

        public async Task<List<Incident>> GetByTripAsync(Guid tripId)
        {
            return await IncidentQuery(asNoTracking: true)
                .Where(incident => incident.TripId == tripId)
                .ToListAsync();
        }

        public async Task<List<Incident>> GetByRouteAssignmentAsync(Guid routeAssignmentId)
        {
            return await IncidentQuery(asNoTracking: true)
                .Where(incident => incident.RouteAssignmentId == routeAssignmentId)
                .ToListAsync();
        }

        public async Task<List<Incident>> GetByReportedByAsync(Guid userId)
        {
            return await IncidentQuery(asNoTracking: true)
                .Where(incident => incident.ReportedByUserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<Incident> IncidentQuery(bool asNoTracking = false)
        {
            var query = _context.Incidents
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .Include(incident => incident.Trip)
                    .ThenInclude(trip => trip!.RouteAssignment)
                        .ThenInclude(assignment => assignment.Route)
                .Include(incident => incident.Trip)
                    .ThenInclude(trip => trip!.RouteAssignment)
                        .ThenInclude(assignment => assignment.Driver)
                .Include(incident => incident.Trip)
                    .ThenInclude(trip => trip!.RouteAssignment)
                        .ThenInclude(assignment => assignment.TransportAssistant)
                .Include(incident => incident.Trip)
                    .ThenInclude(trip => trip!.RouteAssignment)
                        .ThenInclude(assignment => assignment.Students)
                            .ThenInclude(studentAssignment => studentAssignment.Student)
                .Include(incident => incident.RouteAssignment)
                    .ThenInclude(assignment => assignment!.Route)
                .Include(incident => incident.RouteAssignment)
                    .ThenInclude(assignment => assignment!.Students)
                        .ThenInclude(studentAssignment => studentAssignment.Student)
                .Include(incident => incident.ReportedByUser)
                .Include(incident => incident.AssignedToUser)
                .OrderByDescending(incident => incident.CreatedAt);

            return asNoTracking ? query.AsNoTracking() : query;
        }
    }
}
