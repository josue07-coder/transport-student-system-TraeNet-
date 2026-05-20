using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Drivers.Commands.DeleteDriver
{
    public class DeleteDriverHandler : IRequestHandler<DeleteDriverCommand, Unit>
    {
        private readonly IDriverRepository _repository;

        public DeleteDriverHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Conductor no encontrado");

            driver.Deactivate();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
