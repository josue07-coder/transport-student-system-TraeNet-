using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Grades.Commands.DeleteGrade
{
    public class DeleteGradeHandler : IRequestHandler<DeleteGradeCommand, Unit>
    {
        private readonly IGradeRepository _repository;

        public DeleteGradeHandler(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteGradeCommand request, CancellationToken cancellationToken)
        {
            var grade = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Grade not found");

            if (await _repository.HasActiveStudentsAsync(grade.Id))
                throw new DomainException("No se puede eliminar el grado porque tiene estudiantes activos asociados");

            _repository.Delete(grade);
            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
