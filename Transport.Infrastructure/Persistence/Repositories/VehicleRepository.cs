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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
