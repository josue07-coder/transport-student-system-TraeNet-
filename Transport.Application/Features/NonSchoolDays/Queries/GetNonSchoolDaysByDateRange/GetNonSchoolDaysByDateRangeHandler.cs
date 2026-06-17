using MediatR;
using Transport.Application.Features.NonSchoolDays.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetNonSchoolDaysByDateRange
{
    public class GetNonSchoolDaysByDateRangeHandler : IRequestHandler<GetNonSchoolDaysByDateRangeQuery, List<NonSchoolDayResponseDto>>
    {
        private readonly INonSchoolDayRepository _repository;

        public GetNonSchoolDaysByDateRangeHandler(INonSchoolDayRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NonSchoolDayResponseDto>> Handle(GetNonSchoolDaysByDateRangeQuery request, CancellationToken cancellationToken)
        {
            if (request.EndDate < request.StartDate)
                throw new DomainException("La fecha final no puede ser menor que la inicial");

            var days = await _repository.GetByDateRangeAsync(request.StartDate, request.EndDate);
            return days.Select(NonSchoolDayMappings.ToResponseDto).ToList();
        }
    }
}
