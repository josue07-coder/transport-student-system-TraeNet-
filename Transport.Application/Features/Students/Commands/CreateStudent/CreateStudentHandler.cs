using MediatR;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Guid>
    {
        private readonly IStudentRepository _repository;

        public CreateStudentHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            //  Regla de negocio adicional
            if (await _repository.ExistsByCodeAsync(request.Code))
                throw new DomainException("Student with this code already exists");

            var student = new Student(
                request.FirstName,
                request.LastName,
                StudentCode.Create(request.Code),
                request.SchoolId
            );

            await _repository.AddAsync(student);
            await _repository.SaveChangesAsync();

            return student.Id;
        }
    }
}
