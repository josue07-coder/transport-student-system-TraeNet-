using MediatR;
using Transport.Application.Features.Guardians.DTOs;

namespace Transport.Application.Features.Guardians.Queries.GetGuardianByDocument
{
    public record GetGuardianByDocumentQuery(string DocumentNumber) : IRequest<GuardianDetailDto>;
}
