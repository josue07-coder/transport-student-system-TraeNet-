using Transport.Application.DTOs.User;

namespace Transport.Application.UseCases.Users
{
    public interface IcreateUserUserCase
    {
        Task ExecuteAsync(CreateUserDTO dto);
    }
}
