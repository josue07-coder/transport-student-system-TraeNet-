using MediatR;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Queries.GetStudentsByGuardian
{
    public class GetStudentsByGuardianHandler : IRequestHandler<GetStudentsByGuardianQuery, List<StudentResponseDto>>
    {
        private readonly IStudentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetStudentsByGuardianHandler(IStudentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<StudentResponseDto>> Handle(GetStudentsByGuardianQuery request, CancellationToken cancellationToken)
        {
            var students = await _repository.GetByGuardianAsync(request.GuardianId);
            students = await _visibilityService.FilterStudentsAsync(students);

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
