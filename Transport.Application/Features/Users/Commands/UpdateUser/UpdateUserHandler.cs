using MediatR;
using Transport.Application.Features.Users.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDetailDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UpdateUserHandler(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<UserDetailDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithRoleAsync(request.Id)
                ?? throw new DomainException("Usuario no encontrado");

            var role = await _roleRepository.GetByIdAsync(request.RoleId)
                ?? throw new DomainException("Rol no encontrado");

            user.UpdateProfile(request.Name, request.ProfileImageUrl);
            user.SetEmail(request.Email);
            user.ChangeRole(role.Id);

            if (request.IsActive)
                user.Activate();
            else
                user.Deactivate();

            await _userRepository.SaveChangesAsync();

            user = await _userRepository.GetByIdWithRoleAsync(request.Id)
                ?? throw new DomainException("Usuario no encontrado");

            return UserMapper.ToDetailDto(user);
        }
    }
}
