using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.ValueObjects;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class RouteAssignmentRepository : IRouteAssignmentRepository
    {
        private readonly AppDbContext _context;

        public RouteAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RouteAssignment assignment)
        {
            await _context.RouteAssignments.AddAsync(assignment);
        }

        public async Task<RouteAssignment?> GetByIdAsync(Guid id)
        {
            return await AssignmentQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaginatedResponse<RouteAssignment>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = AssignmentQuery(asNoTracking: true)
                .OrderBy(assignment => assignment.Route.Name)
                .ThenBy(assignment => assignment.Driver.LastName)
                .ThenBy(assignment => assignment.Driver.FirstName)
                .ThenBy(assignment => assignment.Id);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<RouteAssignment>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<RouteAssignment>> GetByRouteAsync(Guid routeId)
        {
            return await AssignmentQuery(asNoTracking: true)
                .Where(assignment => assignment.RouteId == routeId)
                .OrderBy(assignment => assignment.Driver.LastName)
                .ThenBy(assignment => assignment.Driver.FirstName)
                .ToListAsync();
        }

        public async Task<List<RouteAssignment>> GetByDriverAsync(Guid driverId)
        {
            return await AssignmentQuery(asNoTracking: true)
                .Where(assignment => assignment.DriverId == driverId)
                .OrderBy(assignment => assignment.Route.Name)
                .ToListAsync();
        }

        public async Task<List<RouteAssignment>> GetByTransportAssistantAsync(Guid transportAssistantId)
        {
            return await AssignmentQuery(asNoTracking: true)
                .Where(assignment => assignment.TransportAssistantId == transportAssistantId)
                .OrderBy(assignment => assignment.Route.Name)
                .ToListAsync();
        }

        public async Task<List<RouteAssignment>> GetByGuardianAsync(Guid guardianId)
        {
            return await AssignmentQuery(asNoTracking: true)
                .Where(assignment => assignment.Students.Any(studentAssignment =>
                    studentAssignment.Student.GuardianId == guardianId))
                .OrderBy(assignment => assignment.Route.Name)
                .ToListAsync();
        }

        public async Task<List<RouteAssignment>> GetByVehicleAsync(Guid vehicleId)
        {
            return await AssignmentQuery(asNoTracking: true)
                .Where(assignment => assignment.VehicleId == vehicleId)
                .OrderBy(assignment => assignment.Route.Name)
                .ToListAsync();
        }

        public async Task<bool> HasTripsAsync(Guid id)
        {
            return await _context.Trips
                .AnyAsync(trip => trip.RouteAssignmentId == id);
        }

        public async Task<bool> HasDriverScheduleConflictAsync(Guid driverId, Guid routeId, Guid? excludedAssignmentId = null)
        {
            return await HasResourceScheduleConflictAsync(
                routeId,
                excludedAssignmentId,
                assignment => assignment.DriverId == driverId);
        }

        public async Task<bool> HasVehicleScheduleConflictAsync(Guid vehicleId, Guid routeId, Guid? excludedAssignmentId = null)
        {
            return await HasResourceScheduleConflictAsync(
                routeId,
                excludedAssignmentId,
                assignment => assignment.VehicleId == vehicleId);
        }

        public async Task<bool> HasTransportAssistantScheduleConflictAsync(Guid transportAssistantId, Guid routeId, Guid? excludedAssignmentId = null)
        {
            return await HasResourceScheduleConflictAsync(
                routeId,
                excludedAssignmentId,
                assignment => assignment.TransportAssistantId == transportAssistantId);
        }

        public async Task<bool> HasStudentScheduleConflictAsync(Guid studentId, Guid routeId, Guid? excludedAssignmentId = null)
        {
            var route = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == routeId);
            if (route is null)
                return false;

            var assignments = await _context.RouteAssignments
                .AsNoTracking()
                .Include(assignment => assignment.Route)
                .Include(assignment => assignment.Students)
                .Where(assignment =>
                    assignment.Route.Status == RouteStatus.Active &&
                    assignment.Students.Any(studentAssignment => studentAssignment.StudentId == studentId))
                .ToListAsync();

            return assignments.Any(assignment =>
                assignment.Id != excludedAssignmentId &&
                assignment.Route.OperatingHours.Overlaps(route.OperatingHours));
        }

        public async Task<int> CountStudentsAsync(Guid id)
        {
            return await _context.StudentRouteAssignments
                .CountAsync(studentAssignment => studentAssignment.RouteAssignmentId == id);
        }

        public void Delete(RouteAssignment assignment)
        {
            _context.RouteAssignments.Remove(assignment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<RouteAssignment> AssignmentQuery(bool asNoTracking = false)
        {
            var query = _context.RouteAssignments
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .Include(x => x.Route)
                    .ThenInclude(route => route.Stops)
                .Include(x => x.Driver)
                .Include(x => x.Vehicle)
                .Include(x => x.TransportAssistant)
                .Include(x => x.Students)
                    .ThenInclude(studentAssignment => studentAssignment.Student)
                .Include(x => x.Trips);

            return asNoTracking ? query.AsNoTracking() : query;
        }

        private async Task<bool> HasResourceScheduleConflictAsync(
            Guid routeId,
            Guid? excludedAssignmentId,
            Func<RouteAssignment, bool> resourcePredicate)
        {
            var route = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == routeId);
            if (route is null)
                return false;

            var assignments = await _context.RouteAssignments
                .AsNoTracking()
                .Include(assignment => assignment.Route)
                .Where(assignment => assignment.Route.Status == RouteStatus.Active)
                .ToListAsync();

            return assignments.Any(assignment =>
                assignment.Id != excludedAssignmentId &&
                resourcePredicate(assignment) &&
                assignment.Route.OperatingHours.Overlaps(route.OperatingHours));
        }
    }
}
