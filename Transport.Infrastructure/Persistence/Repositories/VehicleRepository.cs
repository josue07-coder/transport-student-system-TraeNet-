using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;

        public VehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);
        }

        public async Task<List<Vehicle>> GetByStatusAsync(VehicleStatus status)
        {
            return await _context.Vehicles
                .Where(v => v.Status == status)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<Vehicle>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Vehicles.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Vehicle>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Vehicles
                .AnyAsync(vehicle => vehicle.Id == id);
        }

        public async Task<bool> ExistsByPlateAsync(string plateNumber, Guid? excludeId = null)
        {
            var normalizedPlate = plateNumber.Trim().ToLower();

            return await _context.Vehicles
                .AnyAsync(vehicle =>
                    vehicle.PlateNumber.ToLower() == normalizedPlate &&
                    (!excludeId.HasValue || vehicle.Id != excludeId.Value));
        }

        public async Task<bool> HasInProgressTripAsync(Guid vehicleId)
        {
            return await _context.Trips
                .AnyAsync(trip =>
                    trip.Status == TripStatus.InProgress &&
                    trip.RouteAssignment.VehicleId == vehicleId);
        }

        public async Task<bool> HasActiveRouteAssignmentAsync(Guid vehicleId)
        {
            return await _context.RouteAssignments
                .AnyAsync(assignment =>
                    assignment.VehicleId == vehicleId &&
                    assignment.Route.Status == RouteStatus.Active);
        }

        public async Task<int> GetMaxAssignedStudentCountAsync(Guid vehicleId)
        {
            var counts = await _context.RouteAssignments
                .Where(assignment =>
                    assignment.VehicleId == vehicleId &&
                    assignment.Route.Status == RouteStatus.Active)
                .Select(assignment => assignment.Students.Count)
                .ToListAsync();

            return counts.Count == 0 ? 0 : counts.Max();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
