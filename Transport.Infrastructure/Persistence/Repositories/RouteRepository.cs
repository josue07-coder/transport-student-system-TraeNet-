using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly AppDbContext _context;

        public RouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Route route)
        {
            await _context.Routes.AddAsync(route);
        }

        public void AddRouteStop(RouteStop routeStop)
        {
            _context.RouteStops.Add(routeStop);
        }

        public async Task<Route?> GetByIdAsync(Guid id)
        {
            return await _context.Routes.FindAsync(id);
        }

        public async Task<Route?> GetByIdWithStopsAsync(Guid id)
        {
            return await _context.Routes
                .Include(r => r.Stops)
                    .ThenInclude(routeStop => routeStop.Stop)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> IsActiveAsync(Guid id)
        {
            return await _context.Routes
                .AnyAsync(route => route.Id == id && route.Status == RouteStatus.Active);
        }

        public async Task<bool> HasStopsAsync(Guid id)
        {
            return await _context.RouteStops
                .AnyAsync(routeStop => routeStop.RouteId == id);
        }

        public async Task<bool> HasActiveTripAsync(Guid routeId)
        {
            return await _context.Trips
                .AnyAsync(trip =>
                    trip.Status == TripStatus.InProgress &&
                    trip.RouteAssignment.RouteId == routeId);
        }

        public async Task<bool> ExistsByNameForSchoolAsync(string name, Guid schoolId, Guid? excludeRouteId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Routes
                .AnyAsync(route =>
                    route.SchoolId == schoolId &&
                    route.Name.ToLower() == normalizedName &&
                    (!excludeRouteId.HasValue || route.Id != excludeRouteId.Value));
        }

        public async Task<PaginatedResponse<Route>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Routes
                .AsNoTracking()
                .AsSplitQuery()
                .Include(r => r.Stops)
                .OrderBy(r => r.Name)
                .ThenBy(r => r.Id)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Route>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<Route>> GetBySchoolAsync(Guid schoolId)
        {
            return await _context.Routes
                .AsNoTracking()
                .AsSplitQuery()
                .Include(r => r.Stops)
                .Where(r => r.SchoolId == schoolId)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<Route>> GetByStatusAsync(RouteStatus status)
        {
            return await _context.Routes
                .AsNoTracking()
                .AsSplitQuery()
                .Include(r => r.Stops)
                .Where(r => r.Status == status)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Routes
                .AnyAsync(route => route.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
