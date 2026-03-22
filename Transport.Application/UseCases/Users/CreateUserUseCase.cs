using Transport.Application.Interfaces;
using Transport.Application.DTOs.User;
using Transport.Domain.Entities;

namespace Transport.Application.UseCases.Users
{
    public class CreateUserUseCase: IcreateUserUserCase
    {
        private readonly IUserRepository _userRepository;

        public CreateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task ExecuteAsync(CreateUserDTO dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("El usuario ya existe.");
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = dto.Password,
               
            };
            await _userRepository.Create(user);
        }

    }
}
