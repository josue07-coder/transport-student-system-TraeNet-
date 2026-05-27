using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly AppDbContext _context;

        public SystemSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SystemSetting setting)
        {
            await _context.SystemSettings.AddAsync(setting);
        }

        public Task<List<SystemSetting>> GetAllAsync()
        {
            return _context.SystemSettings
                .AsNoTracking()
                .OrderBy(x => x.Category)
                .ThenBy(x => x.Key)
                .ToListAsync();
        }

        public Task<SystemSetting?> GetByIdAsync(Guid id)
        {
            return _context.SystemSettings.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<SystemSetting?> GetByKeyAsync(string key)
        {
            var normalizedKey = key.Trim();
            return _context.SystemSettings.FirstOrDefaultAsync(x => x.Key == normalizedKey);
        }

        public Task<List<SystemSetting>> GetByCategoryAsync(string category)
        {
            var normalizedCategory = category.Trim();
            return _context.SystemSettings
                .AsNoTracking()
                .Where(x => x.Category == normalizedCategory)
                .OrderBy(x => x.Key)
                .ToListAsync();
        }

        public Task<bool> ExistsByKeyAsync(string key, Guid? excludeId = null)
        {
            var normalizedKey = key.Trim();
            return _context.SystemSettings
                .AnyAsync(x => x.Key == normalizedKey && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public void Remove(SystemSetting setting)
        {
            _context.SystemSettings.Remove(setting);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
