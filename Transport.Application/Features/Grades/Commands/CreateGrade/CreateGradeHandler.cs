using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Grades.Commands.CreateGrade
{
    public class CreateGradeHandler : IRequestHandler<CreateGradeCommand, Guid>
    {
        private readonly IGradeRepository _repo;
        private readonly ISchoolRepository _schoolRepository;

        public CreateGradeHandler(IGradeRepository repo, ISchoolRepository schoolRepository)
        {
            _repo = repo;
            _schoolRepository = schoolRepository;
        }

        public async Task<Guid> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
        {
            if (!await _schoolRepository.IsActiveAsync(request.SchoolId))
                throw new DomainException("La escuela está inactiva o no existe");

            if (await _repo.ExistsByNameInSchoolAsync(request.Name, request.SchoolId))
                throw new DomainException("Ya existe un grado con ese nombre en la escuela");

            var grade = new Grade(request.Name, request.SchoolId);

            await _repo.AddAsync(grade);
            await _repo.SaveChangesAsync();

            return grade.Id;
        }
    }
}
