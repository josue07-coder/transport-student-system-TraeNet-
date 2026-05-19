using MediatR;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Queries.GetStudentsBySchool
{
    public class GetStudentsBySchoolHandler : IRequestHandler<GetStudentsBySchoolQuery, List<StudentResponseDto>>
    {
        private readonly IStudentRepository _repository;

        public GetStudentsBySchoolHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StudentResponseDto>> Handle(GetStudentsBySchoolQuery request, CancellationToken cancellationToken)
        {
            var students = await _repository.GetBySchoolAsync(request.SchoolId);

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
