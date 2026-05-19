using MediatR;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Queries.GetStudentsByGrade
{
    public class GetStudentsByGradeHandler : IRequestHandler<GetStudentsByGradeQuery, List<StudentResponseDto>>
    {
        private readonly IStudentRepository _repository;

        public GetStudentsByGradeHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StudentResponseDto>> Handle(GetStudentsByGradeQuery request, CancellationToken cancellationToken)
        {
            var students = await _repository.GetByGradeAsync(request.GradeId);

            return students.Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FullName = $"{s.FirstName} {s.LastName}",
                SchoolId = s.SchoolId,
                GradeId = s.GradeId,
                GuardianId = s.GuardianId
            }).ToList();
        }
    }
}
