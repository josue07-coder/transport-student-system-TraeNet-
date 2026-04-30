using MediatR;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Queries.GetStudents
{
    public class GetStudentsHandler : IRequestHandler<GetStudentsQuery, List<StudentDto>>
    {
        private readonly IStudentRepository _repository;

        public GetStudentsHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StudentDto>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _repository.GetAllAsync();

            return students.Select(s => new StudentDto
            {
                Id = s.Id,
                FullName = $"{s.FirstName} {s.LastName}",
                Code = s.StudentCode.Value,
                SchoolId = s.SchoolId
            }).ToList();
        }
    }
}
