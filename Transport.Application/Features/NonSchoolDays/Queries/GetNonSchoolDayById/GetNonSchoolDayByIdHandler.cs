using MediatR;
using Transport.Application.Features.NonSchoolDays.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.NonSchoolDays.Queries.GetNonSchoolDayById
{
    public class GetNonSchoolDayByIdHandler : IRequestHandler<GetNonSchoolDayByIdQuery, NonSchoolDayResponseDto>
    {
        private readonly INonSchoolDayRepository _repository;

        public GetNonSchoolDayByIdHandler(INonSchoolDayRepository repository)
        {
            _repository = repository;
        }

        public async Task<NonSchoolDayResponseDto> Handle(GetNonSchoolDayByIdQuery request, CancellationToken cancellationToken)
        {
            var day = await _repository.GetByIdWithSchoolAsync(request.Id)
                ?? throw new DomainException("Día no escolar no encontrado");

            return NonSchoolDayMappings.ToResponseDto(day);
        }
    }
}
