using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Users.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, PaginatedResponse<UserResponseDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PaginatedResponse<UserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = users.Items.Select(UserMapper.ToResponseDto).ToList();
            return new PaginatedResponse<UserResponseDto>(items, users.TotalCount, users.PageNumber, users.PageSize);
        }
    }
}
