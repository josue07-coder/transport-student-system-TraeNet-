using MediatR;
using Transport.Application.Features.Permissions.DTOs;

namespace Transport.Application.Features.Permissions.Queries.GetPermissionById
{
    public class GetPermissionByIdQuery : IRequest<PermissionResponseDto>
    {
        public Guid Id { get; set; }

        public GetPermissionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
