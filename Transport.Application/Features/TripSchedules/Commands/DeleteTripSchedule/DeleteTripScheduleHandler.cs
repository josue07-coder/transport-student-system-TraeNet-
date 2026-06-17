using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripSchedules.Commands.DeleteTripSchedule
{
    public class DeleteTripScheduleHandler : IRequestHandler<DeleteTripScheduleCommand, Unit>
    {
        private readonly ITripScheduleRepository _repository;
        private readonly IAuditService _auditService;

        public DeleteTripScheduleHandler(ITripScheduleRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeleteTripScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Programación de viaje no encontrada");

            if (await _repository.HasTripsAsync(schedule.Id))
            {
                schedule.Deactivate();
                await _repository.SaveChangesAsync();
                await _auditService.LogAsync("TripScheduleDeactivated", "TripSchedule", schedule.Id.ToString(), null, "{\"Reason\":\"Tiene viajes relacionados\"}");
                return Unit.Value;
            }

            _repository.Delete(schedule);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("TripScheduleDeleted", "TripSchedule", schedule.Id.ToString());
            return Unit.Value;
        }
    }
}
