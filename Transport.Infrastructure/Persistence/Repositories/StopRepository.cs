using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class StopRepository : IStopRepository
    {
        private readonly AppDbContext _context;

        public StopRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Stop stop)
        {
            await _context.Stops.AddAsync(stop);
        }

        public async Task<Stop?> GetByIdAsync(Guid id)
        {
            return await _context.Stops.FindAsync(id);
        }

        public async Task<Stop?> GetByIdWithSectorAsync(Guid id)
        {
            return await _context.Stops
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Stop>> GetBySectorAsync(Guid sectorId)
        {
            return await _context.Stops
                .Where(s => s.SectorId == sectorId)
                .ToListAsync();
        }

        public async Task<List<Stop>> GetByCityAsync(string city)
        {
            return await _context.Stops
                .Where(s => s.Address.City == city)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<Stop>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Stops.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Stop>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Stops
                .AnyAsync(stop => stop.Id == id);
        }

        public async Task<bool> HasRouteStopsAsync(Guid stopId)
        {
            return await _context.RouteStops
                .AnyAsync(routeStop => routeStop.StopId == stopId);
        }

        public void Delete(Stop stop)
        {
            _context.Stops.Remove(stop);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
