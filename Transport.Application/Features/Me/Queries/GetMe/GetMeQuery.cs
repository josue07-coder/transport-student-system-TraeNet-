using MediatR;
using Transport.Application.Features.Me.DTOs;

namespace Transport.Application.Features.Me.Queries.GetMe
{
    public class GetMeQuery : IRequest<MeProfileDto>
    {
    }
}
