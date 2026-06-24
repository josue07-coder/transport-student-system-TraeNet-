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
            return await FullTripQuery()
                .FirstOrDefaultAsync(trip => trip.Id == id);
        }

        public async Task<PaginatedResponse<Trip>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = BaseTripQuery()
                .OrderByDescending(trip => trip.StartTime ?? trip.ScheduledDepartureTime ?? trip.CreatedAt)
                .ThenByDescending(trip => trip.CreatedAt)
                .ThenBy(trip => trip.Id);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Trip>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<Trip>> GetByRouteAssignmentAsync(Guid routeAssignmentId)
        {
            return await BaseTripQuery()
                .Where(trip => trip.RouteAssignmentId == routeAssignmentId)
                .OrderByDescending(trip => trip.StartTime ?? trip.ScheduledDepartureTime ?? trip.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByStatusAsync(TripStatus status)
        {
            return await BaseTripQuery()
                .Where(trip => trip.Status == status)
                .OrderByDescending(trip => trip.StartTime ?? trip.ScheduledDepartureTime ?? trip.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await BaseTripQuery()
                .Where(trip => trip.StartTime.HasValue &&
                    trip.StartTime.Value.Date >= startDate.Date &&
                    trip.StartTime.Value.Date <= endDate.Date)
                .OrderByDescending(trip => trip.StartTime)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByDriverAsync(Guid driverId)
        {
            return await VisibilityTripQuery()
                .Where(trip => trip.RouteAssignment.DriverId == driverId)
                .OrderByDescending(trip => trip.StartTime ?? trip.ScheduledDepartureTime ?? trip.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByTransportAssistantAsync(Guid transportAssistantId)
        {
            return await VisibilityTripQuery()
                .Where(trip => trip.RouteAssignment.TransportAssistantId == transportAssistantId)
                .OrderByDescending(trip => trip.StartTime ?? trip.ScheduledDepartureTime ?? trip.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetByGuardianAsync(Guid guardianId)
        {
            return await VisibilityTripQuery()
                .Where(trip => trip.RouteAssignment.Students.Any(studentAssignment =>
                    studentAssignment.Student.GuardianId == guardianId))
                .OrderByDescending(trip => trip.StartTime ?? trip.ScheduledDepartureTime ?? trip.CreatedAt)
                .ToListAsync();
        }

        public async Task<Trip?> GetActiveByRouteAssignmentAsync(Guid routeAssignmentId)
        {
            return await BaseTripQuery()
                .FirstOrDefaultAsync(trip => trip.RouteAssignmentId == routeAssignmentId && trip.Status == TripStatus.InProgress);
        }

        public async Task<Trip?> GetByIdWithAssignmentDetailsAsync(Guid id)
        {
            return await FullTripQuery()
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
            return await VisibilityTripQuery()
                .Where(trip => trip.Status == TripStatus.InProgress)
                .OrderByDescending(trip => trip.StartTime ?? trip.CreatedAt)
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

        private IQueryable<Trip> BaseTripQuery()
        {
            return _context.Trips
                .AsNoTracking();
        }

        private IQueryable<Trip> VisibilityTripQuery()
        {
            return _context.Trips
                .AsNoTracking()
                .AsSplitQuery()
                .Include(trip => trip.RouteAssignment)
                    .ThenInclude(assignment => assignment.Students)
                        .ThenInclude(studentAssignment => studentAssignment.Student);
        }

        private IQueryable<Trip> FullTripQuery()
        {
            return _context.Trips
                .AsSplitQuery()
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
                .Include(trip => trip.RouteDeviations);
        }
    }
}
