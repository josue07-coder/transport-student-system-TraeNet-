using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<PaginatedResponse<User>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users
                .IgnoreQueryFilters()
                .Include(user => user.Role)
                .OrderBy(user => user.Name)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<User>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(user => user.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rolePermission => rolePermission.Permission)
                .FirstOrDefaultAsync(user => user.Username == username);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User?> GetByIdWithRoleAsync(Guid id)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Include(user => user.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rolePermission => rolePermission.Permission)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User?> GetByIdWithLinkedProfilesAsync(Guid id)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Include(user => user.Role)
                .Include(user => user.Guardian)
                .Include(user => user.Driver)
                .Include(user => user.TransportAssistant)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<List<User>> GetByRoleAsync(Guid roleId)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Include(user => user.Role)
                .Where(user => user.RoleId == roleId)
                .OrderBy(user => user.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetByActiveAsync(bool isActive)
        {
            var query = _context.Users
                .IgnoreQueryFilters()
                .Include(user => user.Role)
                .Where(user => user.IsActive == isActive);

            return await query
                .OrderBy(user => user.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .AnyAsync(user => user.Username == username);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
