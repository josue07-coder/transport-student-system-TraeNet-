using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.NonSchoolDays.DTOs;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetAllNonSchoolDays
{
    public class GetAllNonSchoolDaysQuery : PaginationRequest, IRequest<PaginatedResponse<NonSchoolDayResponseDto>>
    {
    }
}
