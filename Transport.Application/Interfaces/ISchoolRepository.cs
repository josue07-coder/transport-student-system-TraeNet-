

using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ISchoolRepository
    {
        Task AddAsync(School school);
        Task<School?> GetByIdAsync(Guid id);
        Task<List<School>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
