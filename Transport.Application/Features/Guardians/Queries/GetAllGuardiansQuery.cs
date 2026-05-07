using MediatR;
using Transport.Application.Features.Guardians.DTOs;

public class GetAllGuardiansQuery : IRequest<List<GuardianResponseDto>>
{
}
