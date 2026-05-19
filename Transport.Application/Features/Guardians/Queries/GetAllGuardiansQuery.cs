using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Guardians.DTOs;

public class GetAllGuardiansQuery : PaginationRequest, IRequest<PaginatedResponse<GuardianResponseDto>>
{
}
