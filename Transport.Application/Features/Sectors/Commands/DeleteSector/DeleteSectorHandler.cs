using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Sectors.Commands.DeleteSector
{
    public class DeleteSectorHandler : IRequestHandler<DeleteSectorCommand, Unit>
    {
        private readonly ISectorRepository _repository;

        public DeleteSectorHandler(ISectorRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteSectorCommand request, CancellationToken cancellationToken)
        {
            var sector = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Sector not found");

            if (await _repository.HasSchoolsAsync(sector.Id))
                throw new DomainException("No se puede eliminar el sector porque tiene escuelas asociadas");

            if (await _repository.HasGuardiansAsync(sector.Id))
                throw new DomainException("No se puede eliminar el sector porque tiene tutores asociados");

            if (await _repository.HasStopsAsync(sector.Id))
                throw new DomainException("No se puede eliminar el sector porque tiene paradas asociadas");

            _repository.Delete(sector);
            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
