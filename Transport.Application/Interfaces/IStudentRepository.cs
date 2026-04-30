using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task AddAsync(Student student);
        Task <Student> GetByIdAsync (Guid id);
        Task<List<Student>> GetAllAsync();
        Task<bool> ExistsByCodeAsync(string code);
        Task SaveChangesAsync();
    }
}
