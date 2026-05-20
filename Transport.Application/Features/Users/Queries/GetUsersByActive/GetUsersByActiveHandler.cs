using MediatR;
using Transport.Application.Features.Users.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Users.Queries.GetUsersByActive
{
    public class GetUsersByActiveHandler : IRequestHandler<GetUsersByActiveQuery, List<UserResponseDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersByActiveHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserResponseDto>> Handle(GetUsersByActiveQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetByActiveAsync(request.IsActive);
            return users.Select(UserMapper.ToResponseDto).ToList();
        }
    }
}
