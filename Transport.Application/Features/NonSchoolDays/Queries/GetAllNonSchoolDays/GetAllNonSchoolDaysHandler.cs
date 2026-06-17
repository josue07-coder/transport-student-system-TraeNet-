using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.NonSchoolDays.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetAllNonSchoolDays
{
    public class GetAllNonSchoolDaysHandler : IRequestHandler<GetAllNonSchoolDaysQuery, PaginatedResponse<NonSchoolDayResponseDto>>
    {
        private readonly INonSchoolDayRepository _repository;

        public GetAllNonSchoolDaysHandler(INonSchoolDayRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<NonSchoolDayResponseDto>> Handle(GetAllNonSchoolDaysQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            return new PaginatedResponse<NonSchoolDayResponseDto>(
                result.Items.Select(NonSchoolDayMappings.ToResponseDto),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);
        }
    }
}
