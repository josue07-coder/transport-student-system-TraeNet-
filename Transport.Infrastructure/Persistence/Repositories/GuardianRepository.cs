using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence;
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
            return await _context.Guardians.FindAsync(id);
        }
        public async Task<List<Guardian>> GetAllAsync()
        {
            return await _context.Guardians.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}