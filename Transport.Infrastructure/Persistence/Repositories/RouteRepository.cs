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

        public async Task<PaginatedResponse<Route>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Routes
                .Include(r => r.Stops)
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
                .Include(r => r.Stops)
                .Where(r => r.SchoolId == schoolId)
                .ToListAsync();
        }

        public async Task<List<Route>> GetByStatusAsync(RouteStatus status)
        {
            return await _context.Routes
                .Include(r => r.Stops)
                .Where(r => r.Status == status)
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
