using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly AppDbContext _context;

        public TripRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Trip trip)
        {
            await _context.Trips.AddAsync(trip);
        }

        public async Task<Trip?> GetByIdAsync(Guid id)
        {
            return await TripQuery()
                .FirstOrDefaultAsync(trip => trip.Id == id);
        }

        public async Task<PaginatedResponse<Trip>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = TripQuery();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Trip>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<Trip>> GetByRouteAssignmentAsync(Guid routeAssignmentId)
        {
            return await TripQuery()
                .Where(trip => trip.RouteAssignmentId == routeAssignmentId)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByStatusAsync(TripStatus status)
        {
            return await TripQuery()
                .Where(trip => trip.Status == status)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await TripQuery()
                .Where(trip => trip.StartTime.HasValue &&
                    trip.StartTime.Value.Date >= startDate.Date &&
                    trip.StartTime.Value.Date <= endDate.Date)
                .ToListAsync();
        }

        public async Task<Trip?> GetActiveByRouteAssignmentAsync(Guid routeAssignmentId)
        {
            return await TripQuery()
                .FirstOrDefaultAsync(trip => trip.RouteAssignmentId == routeAssignmentId && trip.Status == TripStatus.InProgress);
        }

        public async Task<bool> HasActiveTripAsync(Guid routeAssignmentId)
        {
            return await _context.Trips
                .AnyAsync(trip => trip.RouteAssignmentId == routeAssignmentId && trip.Status == TripStatus.InProgress);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<Trip> TripQuery()
        {
            return _context.Trips
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.Route)
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.Driver)
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.Vehicle)
                .AsQueryable();
        }
    }
}
