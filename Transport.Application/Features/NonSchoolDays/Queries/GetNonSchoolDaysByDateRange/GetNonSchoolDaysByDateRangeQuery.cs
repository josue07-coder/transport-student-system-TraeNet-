using MediatR;
using Transport.Application.Features.NonSchoolDays.DTOs;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetNonSchoolDaysByDateRange
{
    public class GetNonSchoolDaysByDateRangeQuery : IRequest<List<NonSchoolDayResponseDto>>
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
