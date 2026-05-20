using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByIdWithPermissionsAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task AddAsync(Role role);
        Task<bool> ExistsByNameAsync(string name);
        Task SaveChangesAsync();
    }
}
