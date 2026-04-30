using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class RouteAssignmentRepository : IRouteAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public RouteAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RouteAssignment?> GetByIdAsync(Guid id)
        {
            return await _context.RouteAssignments.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}