using MediatR;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Me.Queries.GetMyStudents
{
    public class GetMyStudentsHandler : IRequestHandler<GetMyStudentsQuery, List<StudentResponseDto>>
    {
        private readonly IVisibilityService _visibilityService;
        private readonly IStudentRepository _studentRepository;

        public GetMyStudentsHandler(IVisibilityService visibilityService, IStudentRepository studentRepository)
        {
            _visibilityService = visibilityService;
            _studentRepository = studentRepository;
        }

        public async Task<List<StudentResponseDto>> Handle(GetMyStudentsQuery request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            var students = user.GuardianId.HasValue
                ? await _studentRepository.GetByGuardianAsync(user.GuardianId.Value)
                : new List<Student>();

            students = await _visibilityService.FilterStudentsAsync(students);

            return students.Select(student => new StudentResponseDto
            {
                Id = student.Id,
                FullName = $"{student.FirstName} {student.LastName}",
                SchoolId = student.SchoolId,
                GradeId = student.GradeId,
                GuardianId = student.GuardianId
            }).ToList();
        }
    }
}
