using MediatR;
using Transport.Application.Features.NonSchoolDays.DTOs;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetNonSchoolDayById
{
    public class GetNonSchoolDayByIdQuery : IRequest<NonSchoolDayResponseDto>
    {
        public Guid Id { get; }

        public GetNonSchoolDayByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
