using MediatR;
using Transport.Application.Features.Grades.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Grades.Queries.GetGradeById
{
    public class GetGradeByIdHandler : IRequestHandler<GetGradeByIdQuery, GradeResponseDto>
    {
        private readonly IGradeRepository _repository;

        public GetGradeByIdHandler(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<GradeResponseDto> Handle(GetGradeByIdQuery request, CancellationToken cancellationToken)
        {
            var grade = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Grade not found");

            return new GradeResponseDto
            {
                Id = grade.Id,
                Name = grade.Name,
                SchoolId = grade.SchoolId
            };
        }
    }
}
