using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly AppDbContext _context;

        public DriverRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Driver driver)
        {
            await _context.Drivers.AddAsync(driver);
        }

        public async Task<Driver?> GetByIdAsync(Guid id)
        {
            return await _context.Drivers.FindAsync(id);
        }

        public async Task<Driver?> GetByIdIncludingInactiveAsync(Guid id)
        {
            return await _context.Drivers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(driver => driver.Id == id);
        }

        public async Task<Driver?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.Drivers
                .FirstOrDefaultAsync(d => d.LicenseNumber.Value == licenseNumber);
        }

        public async Task<List<Driver>> GetByActiveAsync(bool isActive)
        {
            return await _context.Drivers
                .IgnoreQueryFilters()
                .Where(d => d.IsActive == isActive)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<Driver>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Drivers.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Driver>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Drivers
                .AnyAsync(driver => driver.Id == id);
        }

        public async Task<bool> ExistsByDocumentAsync(string documentNumber, Guid? excludeId = null)
        {
            var normalizedDocument = documentNumber.Trim().ToLower();

            return await _context.Drivers
                .IgnoreQueryFilters()
                .AnyAsync(driver =>
                    driver.DocumentNumber.ToLower() == normalizedDocument &&
                    (!excludeId.HasValue || driver.Id != excludeId.Value));
        }

        public async Task<bool> ExistsByLicenseAsync(string licenseNumber, Guid? excludeId = null)
        {
            var normalizedLicense = licenseNumber.Trim().ToLower();

            return await _context.Drivers
                .IgnoreQueryFilters()
                .AnyAsync(driver =>
                    driver.LicenseNumber.Value.ToLower() == normalizedLicense &&
                    (!excludeId.HasValue || driver.Id != excludeId.Value));
        }

        public async Task<bool> HasInProgressTripAsync(Guid driverId)
        {
            return await _context.Trips
                .AnyAsync(trip =>
                    trip.Status == TripStatus.InProgress &&
                    trip.RouteAssignment.DriverId == driverId);
        }

        public async Task<bool> HasActiveRouteAssignmentAsync(Guid driverId)
        {
            return await _context.RouteAssignments
                .AnyAsync(assignment =>
                    assignment.DriverId == driverId &&
                    assignment.Route.Status == RouteStatus.Active);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
