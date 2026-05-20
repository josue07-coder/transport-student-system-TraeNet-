using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByIdWithRoleAsync(Guid id);
        Task<User?> GetByIdWithLinkedProfilesAsync(Guid id);
        Task<bool> ExistsByUsernameAsync(string username);
        Task SaveChangesAsync();
    }
}
