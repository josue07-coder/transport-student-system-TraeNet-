using MediatR;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Features.Students.Queries.GetStudents;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, List<StudentResponseDto>>
    {
        private readonly IStudentRepository _repo;

        public GetAllStudentsHandler(IStudentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<StudentResponseDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _repo.GetAllAsync();

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