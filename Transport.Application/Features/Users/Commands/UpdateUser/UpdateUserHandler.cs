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
        private readonly IAuditService _auditService;

        public UpdateUserHandler(IUserRepository userRepository, IRoleRepository roleRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _auditService = auditService;
        }

        public async Task<UserDetailDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithRoleAsync(request.Id)
                ?? throw new DomainException("Usuario no encontrado");

            var role = await _roleRepository.GetByIdAsync(request.RoleId)
                ?? throw new DomainException("Rol no encontrado");

            var oldValues = $"{{\"Name\":\"{user.Name}\",\"Email\":\"{user.Email}\",\"RoleId\":\"{user.RoleId}\",\"IsActive\":{user.IsActive.ToString().ToLowerInvariant()}}}";

            user.UpdateProfile(request.Name, request.ProfileImageUrl);
            user.SetEmail(request.Email);
            user.ChangeRole(role.Id);

            if (request.IsActive)
                user.Activate();
            else
                user.Deactivate();

            await _userRepository.SaveChangesAsync();

            var newValues = $"{{\"Name\":\"{user.Name}\",\"Email\":\"{user.Email}\",\"RoleId\":\"{user.RoleId}\",\"IsActive\":{user.IsActive.ToString().ToLowerInvariant()}}}";
            await _auditService.LogAsync("Updated", "User", user.Id.ToString(), oldValues, newValues);

            user = await _userRepository.GetByIdWithRoleAsync(request.Id)
                ?? throw new DomainException("Usuario no encontrado");

            return UserMapper.ToDetailDto(user);
        }
    }
}
