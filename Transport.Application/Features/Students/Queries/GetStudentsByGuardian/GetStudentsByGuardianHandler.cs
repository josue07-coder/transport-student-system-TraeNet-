using MediatR;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Queries.GetStudentsByGuardian
{
    public class GetStudentsByGuardianHandler : IRequestHandler<GetStudentsByGuardianQuery, List<StudentResponseDto>>
    {
        private readonly IStudentRepository _repository;

        public GetStudentsByGuardianHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StudentResponseDto>> Handle(GetStudentsByGuardianQuery request, CancellationToken cancellationToken)
        {
            var students = await _repository.GetByGuardianAsync(request.GuardianId);

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
