using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Schools.DTOs;

namespace Transport.Application.Features.Schools.Queries.GetAllSchools
{
    public class GetAllSchoolsQuery : PaginationRequest, IRequest<PaginatedResponse<SchoolResponseDto>>
    {
    }
}
