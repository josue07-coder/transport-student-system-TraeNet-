using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Repositories
{
    public class GuardianRepository : IGuardianRepository
    {
        private readonly AppDbContext _context;

        public GuardianRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Guardian guardian)
        {
            await _context.Guardians.AddAsync(guardian);
        }

        public async Task<Guardian?> GetByIdAsync(Guid id)
        {
            return await _context.Guardians
                .Include(g => g.Sector)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Guardian?> GetByDocumentAsync(string documentNumber)
        {
            return await _context.Guardians
                .Include(g => g.Sector)
                .FirstOrDefaultAsync(g => g.DocumentNumber == documentNumber);
        }

        public async Task<List<Guardian>> GetAllAsync()
        {
            return await _context.Guardians.ToListAsync();
        }

        public async Task<PaginatedResponse<Guardian>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Guardians.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Guardian>(items, totalCount, pageNumber, pageSize);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
