using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Sectors.DTOs;

public class GetAllSectorsQuery : PaginationRequest, IRequest<PaginatedResponse<SectorResponseDto>>
{
}
