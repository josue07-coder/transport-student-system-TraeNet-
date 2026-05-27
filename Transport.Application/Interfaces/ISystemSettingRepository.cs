using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ISystemSettingRepository
    {
        Task AddAsync(SystemSetting setting);
        Task<List<SystemSetting>> GetAllAsync();
        Task<SystemSetting?> GetByIdAsync(Guid id);
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task<List<SystemSetting>> GetByCategoryAsync(string category);
        Task<bool> ExistsByKeyAsync(string key, Guid? excludeId = null);
        void Remove(SystemSetting setting);
        Task SaveChangesAsync();
    }
}
