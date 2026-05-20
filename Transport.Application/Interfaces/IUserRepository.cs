using Transport.Domain.Entities;
using Transport.Application.Common.Pagination;

namespace Transport.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<PaginatedResponse<User>> GetPagedAsync(int pageNumber, int pageSize);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByIdWithRoleAsync(Guid id);
        Task<User?> GetByIdWithLinkedProfilesAsync(Guid id);
        Task<List<User>> GetByRoleAsync(Guid roleId);
        Task<List<User>> GetByActiveAsync(bool isActive);
        Task<bool> ExistsByUsernameAsync(string username);
        Task SaveChangesAsync();
    }
}
