using MediatR;

using Transport.Application.Features.Students.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Students.Queries.GetStudentById
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, StudentDetailDto>
    {
        private readonly IStudentRepository _repository;

        public GetStudentByIdHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<StudentDetailDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Student not found");

            return new StudentDetailDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                FullName = $"{student.FirstName} {student.LastName}",
                StudentCode = student.StudentCode.Value,
                SchoolId = student.SchoolId,
                SchoolName = student.School?.Name,
                GradeId = student.GradeId,
                GradeName = student.Grade?.Name,
                GuardianId = student.GuardianId,
                GuardianName = student.Guardian == null
                    ? null
                    : $"{student.Guardian.FirstName} {student.Guardian.LastName}",
                PhotoUrl = student.PhotoUrl,
                IsActive = student.IsActive
            };
        }
    }
}
