using MediatR;
using Transport.Application.Features.Guardians.DTOs;

namespace Transport.Application.Features.Guardians.Queries.GetGuardianById
{
    public record GetGuardianByIdQuery(Guid Id) : IRequest<GuardianDetailDto>;
}
