using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task AddAsync(Role role);
        Task<bool> ExistsByNameAsync(string name);
        Task SaveChangesAsync();
    }
}
