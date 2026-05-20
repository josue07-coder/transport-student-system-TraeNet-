using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, string roleName);
    }
}
