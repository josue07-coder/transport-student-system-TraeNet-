using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Users.DTOs;

namespace Transport.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : PaginationRequest, IRequest<PaginatedResponse<UserResponseDto>>
    {
    }
}
