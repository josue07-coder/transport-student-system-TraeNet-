using MediatR;
using Transport.Application.Features.Users.DTOs;

namespace Transport.Application.Features.Users.Queries.GetUsersByActive
{
    public class GetUsersByActiveQuery : IRequest<List<UserResponseDto>>
    {
        public bool IsActive { get; set; }

        public GetUsersByActiveQuery(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
