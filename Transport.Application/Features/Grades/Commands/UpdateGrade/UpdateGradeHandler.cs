using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Grades.Commands.UpdateGrade
{
    public class UpdateGradeHandler : IRequestHandler<UpdateGradeCommand, Unit>
    {
        private readonly IGradeRepository _repository;
        private readonly ISchoolRepository _schoolRepository;

        public UpdateGradeHandler(IGradeRepository repository, ISchoolRepository schoolRepository)
        {
            _repository = repository;
            _schoolRepository = schoolRepository;
        }

        public async Task<Unit> Handle(UpdateGradeCommand request, CancellationToken cancellationToken)
        {
            var grade = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Grade not found");

            if (!await _schoolRepository.IsActiveAsync(grade.SchoolId))
                throw new DomainException("La escuela está inactiva o no existe");

            if (await _repository.ExistsByNameInSchoolAsync(request.Name, grade.SchoolId, grade.Id))
                throw new DomainException("Ya existe un grado con ese nombre en la escuela");

            grade.UpdateName(request.Name);
            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
