using MediatR;
using Transport.Application.Features.NonSchoolDays.DTOs;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetActiveNonSchoolDayForDate
{
    public class GetActiveNonSchoolDayForDateQuery : IRequest<NonSchoolDayResponseDto?>
    {
        public DateOnly Date { get; set; }
        public Guid? SchoolId { get; set; }
    }
}
