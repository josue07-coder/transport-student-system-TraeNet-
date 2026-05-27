using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Schools.Commands.DeleteSchool
{
    public class DeleteSchoolHandler : IRequestHandler<DeleteSchoolCommand, Unit>
    {
        private readonly ISchoolRepository _repository;

        public DeleteSchoolHandler(ISchoolRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
        {
            var school = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("School not found");

            if (await _repository.HasActiveStudentsAsync(school.Id))
                throw new DomainException("No se puede desactivar la escuela porque tiene estudiantes activos");

            if (await _repository.HasActiveRoutesAsync(school.Id))
                throw new DomainException("No se puede desactivar la escuela porque tiene rutas activas");

            school.Deactivate();
            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
