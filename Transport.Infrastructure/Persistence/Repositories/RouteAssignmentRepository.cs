using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
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
            var query = AssignmentQuery();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<RouteAssignment>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<RouteAssignment>> GetByRouteAsync(Guid routeId)
        {
            return await AssignmentQuery()
                .Where(assignment => assignment.RouteId == routeId)
                .ToListAsync();
        }

        public async Task<List<RouteAssignment>> GetByDriverAsync(Guid driverId)
        {
            return await AssignmentQuery()
                .Where(assignment => assignment.DriverId == driverId)
                .ToListAsync();
        }

        public async Task<List<RouteAssignment>> GetByVehicleAsync(Guid vehicleId)
        {
            return await AssignmentQuery()
                .Where(assignment => assignment.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task<bool> HasTripsAsync(Guid id)
        {
            return await _context.Trips
                .AnyAsync(trip => trip.RouteAssignmentId == id);
        }

        public void Delete(RouteAssignment assignment)
        {
            _context.RouteAssignments.Remove(assignment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<RouteAssignment> AssignmentQuery()
        {
            return _context.RouteAssignments
                .Include(x => x.Route)
                .Include(x => x.Driver)
                .Include(x => x.Vehicle)
                .Include(x => x.TransportAssistant)
                .Include(x => x.Students)
                    .ThenInclude(studentAssignment => studentAssignment.Student)
                .Include(x => x.Trips)
                .AsQueryable();
        }
    }
}
