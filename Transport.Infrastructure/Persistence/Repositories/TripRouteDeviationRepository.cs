using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class TripRouteDeviationRepository : ITripRouteDeviationRepository
    {
        private readonly AppDbContext _context;

        public TripRouteDeviationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TripRouteDeviation deviation)
        {
            await _context.TripRouteDeviations.AddAsync(deviation);
        }

        public async Task<TripRouteDeviation?> GetByIdAsync(Guid id)
        {
            return await DeviationQuery()
                .FirstOrDefaultAsync(deviation => deviation.Id == id);
        }

        public async Task<List<TripRouteDeviation>> GetByTripAsync(Guid tripId)
        {
            return await DeviationQuery()
                .Where(deviation => deviation.TripId == tripId)
                .OrderByDescending(deviation => deviation.ReportedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<TripRouteDeviation> DeviationQuery()
        {
            return _context.TripRouteDeviations
                .Include(deviation => deviation.ReportedByUser)
                .Include(deviation => deviation.Trip)
                    .ThenInclude(trip => trip.RouteAssignment)
                        .ThenInclude(assignment => assignment.Route)
                .Include(deviation => deviation.Trip)
                    .ThenInclude(trip => trip.RouteAssignment)
                        .ThenInclude(assignment => assignment.Driver)
                .Include(deviation => deviation.Trip)
                    .ThenInclude(trip => trip.RouteAssignment)
                        .ThenInclude(assignment => assignment.TransportAssistant)
                .AsQueryable();
        }
    }
}
