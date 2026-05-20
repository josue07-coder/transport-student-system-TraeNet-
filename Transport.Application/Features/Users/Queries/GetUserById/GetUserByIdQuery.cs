using MediatR;
using Transport.Application.Features.Users.DTOs;

namespace Transport.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDetailDto>
    {
        public Guid Id { get; set; }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
