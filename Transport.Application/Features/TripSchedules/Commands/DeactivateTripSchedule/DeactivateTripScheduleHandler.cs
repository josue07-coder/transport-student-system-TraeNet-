using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripSchedules.Commands.DeactivateTripSchedule
{
    public class DeactivateTripScheduleHandler : IRequestHandler<DeactivateTripScheduleCommand, Unit>
    {
        private readonly ITripScheduleRepository _repository;
        private readonly IAuditService _auditService;

        public DeactivateTripScheduleHandler(ITripScheduleRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeactivateTripScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Programación de viaje no encontrada");

            schedule.Deactivate();
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("TripScheduleDeactivated", "TripSchedule", schedule.Id.ToString());
            return Unit.Value;
        }
    }
}
