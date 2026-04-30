using MediatR;

using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Students.Queries.GetStudentById
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, StudentDto>
    {
        private readonly IStudentRepository _repository;

        public GetStudentByIdHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Student not found");

            return new StudentDto
            {
                Id = student.Id,
                FullName = $"{student.FirstName} {student.LastName}",
                Code = student.StudentCode.Value,
                SchoolId = student.SchoolId
            };
        }
    }
}
