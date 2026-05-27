using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class VehicleLocationRepository : IVehicleLocationRepository
    {
        private readonly AppDbContext _context;

        public VehicleLocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VehicleLocation location)
        {
            await _context.VehicleLocations.AddAsync(location);
        }

        public async Task<VehicleLocation?> GetLatestByTripAsync(Guid tripId)
        {
            return await LocationQuery()
                .Where(location => location.TripId == tripId)
                .OrderByDescending(location => location.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<VehicleLocation>> GetHistoryByTripAsync(Guid tripId)
        {
            return await LocationQuery()
                .Where(location => location.TripId == tripId)
                .OrderByDescending(location => location.RecordedAt)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<VehicleLocation>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = LocationQuery().OrderByDescending(location => location.RecordedAt);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<VehicleLocation>(items, totalCount, pageNumber, pageSize);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<VehicleLocation> LocationQuery()
        {
            return _context.VehicleLocations
                .Include(location => location.Trip)
                    .ThenInclude(trip => trip.RouteAssignment)
                        .ThenInclude(assignment => assignment.Route)
                .Include(location => location.Trip)
                    .ThenInclude(trip => trip.RouteAssignment)
                        .ThenInclude(assignment => assignment.Driver)
                .Include(location => location.Trip)
                    .ThenInclude(trip => trip.RouteAssignment)
                        .ThenInclude(assignment => assignment.TransportAssistant)
                .Include(location => location.Vehicle)
                .AsQueryable();
        }
    }
}
