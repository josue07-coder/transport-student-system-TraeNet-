using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IGuardianRepository
    {
        Task AddAsync(Guardian guardian);
        Task<Guardian?> GetByIdAsync(Guid id);
        Task<List<Guardian>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
