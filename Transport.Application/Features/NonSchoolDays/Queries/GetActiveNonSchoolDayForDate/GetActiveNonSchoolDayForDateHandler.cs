using MediatR;
using Transport.Application.Features.NonSchoolDays.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetActiveNonSchoolDayForDate
{
    public class GetActiveNonSchoolDayForDateHandler : IRequestHandler<GetActiveNonSchoolDayForDateQuery, NonSchoolDayResponseDto?>
    {
        private readonly INonSchoolDayRepository _repository;

        public GetActiveNonSchoolDayForDateHandler(INonSchoolDayRepository repository)
        {
            _repository = repository;
        }

        public async Task<NonSchoolDayResponseDto?> Handle(GetActiveNonSchoolDayForDateQuery request, CancellationToken cancellationToken)
        {
            var day = await _repository.GetActiveForDateAsync(request.Date, request.SchoolId);
            return day is null ? null : NonSchoolDayMappings.ToResponseDto(day);
        }
    }
}
