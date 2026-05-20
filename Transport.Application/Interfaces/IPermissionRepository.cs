using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllAsync();
        Task<Permission?> GetByIdAsync(Guid id);
        Task<Permission?> GetByNameAsync(string name);
        Task<bool> ExistsAsync(Guid id);
    }
}
