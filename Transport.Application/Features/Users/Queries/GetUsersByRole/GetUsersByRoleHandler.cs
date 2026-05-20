using MediatR;
using Transport.Application.Features.Users.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Users.Queries.GetUsersByRole
{
    public class GetUsersByRoleHandler : IRequestHandler<GetUsersByRoleQuery, List<UserResponseDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersByRoleHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserResponseDto>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetByRoleAsync(request.RoleId);
            return users.Select(UserMapper.ToResponseDto).ToList();
        }
    }
}
