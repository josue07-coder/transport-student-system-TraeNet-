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

        public async Task<List<Trip>> GetByDriverAsync(Guid driverId)
        {
            return await TripQuery()
                .Where(trip => trip.RouteAssignment.DriverId == driverId)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByTransportAssistantAsync(Guid transportAssistantId)
        {
            return await TripQuery()
                .Where(trip => trip.RouteAssignment.TransportAssistantId == transportAssistantId)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByGuardianAsync(Guid guardianId)
        {
            return await TripQuery()
                .Where(trip => trip.RouteAssignment.Students.Any(studentAssignment =>
                    studentAssignment.Student.GuardianId == guardianId))
                .ToListAsync();
        }

        public async Task<Trip?> GetActiveByRouteAssignmentAsync(Guid routeAssignmentId)
        {
            return await TripQuery()
                .FirstOrDefaultAsync(trip => trip.RouteAssignmentId == routeAssignmentId && trip.Status == TripStatus.InProgress);
        }

        public async Task<Trip?> GetByIdWithAssignmentDetailsAsync(Guid id)
        {
            return await TripQuery()
                .FirstOrDefaultAsync(trip => trip.Id == id);
        }

        public async Task<bool> ExistsByScheduleAndDateAsync(Guid tripScheduleId, DateOnly operationDate)
        {
            return await _context.Trips
                .AnyAsync(trip => trip.TripScheduleId == tripScheduleId && trip.OperationDate == operationDate);
        }

        public async Task<bool> IsInProgressAsync(Guid id)
        {
            return await _context.Trips
                .AnyAsync(trip => trip.Id == id && trip.Status == TripStatus.InProgress);
        }

        public async Task<List<Trip>> GetActiveTripsAsync()
        {
            return await TripQuery()
                .Where(trip => trip.Status == TripStatus.InProgress)
                .ToListAsync();
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
                        .ThenInclude(route => route.Stops)
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.Driver)
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.Vehicle)
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.TransportAssistant)
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.Students)
                        .ThenInclude(studentAssignment => studentAssignment.Student)
                            .ThenInclude(student => student.Guardian)
                .Include(trip => trip.StudentAttendances)
                .AsQueryable();
        }
    }
}
