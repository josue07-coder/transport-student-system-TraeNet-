using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class TripScheduleRepository : ITripScheduleRepository
    {
        private readonly AppDbContext _context;

        public TripScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TripSchedule schedule)
        {
            await _context.TripSchedules.AddAsync(schedule);
        }

        public async Task<List<TripSchedule>> GetAllAsync()
        {
            return await ScheduleQuery().ToListAsync();
        }

        public async Task<TripSchedule?> GetByIdAsync(Guid id)
        {
            return await _context.TripSchedules
                .FirstOrDefaultAsync(schedule => schedule.Id == id);
        }

        public async Task<TripSchedule?> GetByIdWithDetailsAsync(Guid id)
        {
            return await ScheduleQuery()
                .FirstOrDefaultAsync(schedule => schedule.Id == id);
        }

        public async Task<TripSchedule?> GetByIdWithAssignmentDetailsAsync(Guid id)
        {
            return await ScheduleQuery()
                .FirstOrDefaultAsync(schedule => schedule.Id == id);
        }

        public async Task<List<TripSchedule>> GetByAssignmentAsync(Guid routeAssignmentId)
        {
            return await ScheduleQuery()
                .Where(schedule => schedule.RouteAssignmentId == routeAssignmentId)
                .ToListAsync();
        }

        public async Task<List<TripSchedule>> GetActiveByAssignmentAsync(Guid routeAssignmentId)
        {
            return await ScheduleQuery()
                .Where(schedule => schedule.RouteAssignmentId == routeAssignmentId && schedule.IsActive)
                .ToListAsync();
        }

        public async Task<bool> ExistsOverlapAsync(
            Guid routeAssignmentId,
            TripDirection direction,
            DateOnly validFrom,
            DateOnly? validTo,
            bool monday,
            bool tuesday,
            bool wednesday,
            bool thursday,
            bool friday,
            bool saturday,
            bool sunday,
            Guid? excludeId = null)
        {
            var end = validTo ?? DateOnly.MaxValue;

            return await _context.TripSchedules.AnyAsync(schedule =>
                schedule.Id != excludeId &&
                schedule.IsActive &&
                schedule.RouteAssignmentId == routeAssignmentId &&
                schedule.Direction == direction &&
                schedule.ValidFrom <= end &&
                (schedule.ValidTo ?? DateOnly.MaxValue) >= validFrom &&
                ((monday && schedule.Monday) ||
                 (tuesday && schedule.Tuesday) ||
                 (wednesday && schedule.Wednesday) ||
                 (thursday && schedule.Thursday) ||
                 (friday && schedule.Friday) ||
                 (saturday && schedule.Saturday) ||
                 (sunday && schedule.Sunday)));
        }

        public async Task<bool> HasTripsAsync(Guid id)
        {
            return await _context.Trips.AnyAsync(trip => trip.TripScheduleId == id);
        }

        public void Delete(TripSchedule schedule)
        {
            _context.TripSchedules.Remove(schedule);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<TripSchedule> ScheduleQuery()
        {
            return _context.TripSchedules
                .Include(schedule => schedule.RouteAssignment)
                    .ThenInclude(assignment => assignment.Route)
                .Include(schedule => schedule.RouteAssignment)
                    .ThenInclude(assignment => assignment.Driver)
                .Include(schedule => schedule.RouteAssignment)
                    .ThenInclude(assignment => assignment.Vehicle)
                .Include(schedule => schedule.RouteAssignment)
                    .ThenInclude(assignment => assignment.TransportAssistant)
                .AsQueryable();
        }
    }
}
