using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IGradeRepository
    {
        Task AddAsync(Grade grade);
        Task<Grade?> GetByIdAsync(Guid id);
        Task<List<Grade>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
