using Transport.Domain.Entities;

namespace Transport.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetRoleByIdAsync(Guid id);
        Task<User> GetRoleByNameAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
    }
}
