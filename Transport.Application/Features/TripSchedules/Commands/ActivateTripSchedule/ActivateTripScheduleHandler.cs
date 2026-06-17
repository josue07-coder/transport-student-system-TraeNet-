using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripSchedules.Commands.ActivateTripSchedule
{
    public class ActivateTripScheduleHandler : IRequestHandler<ActivateTripScheduleCommand, Unit>
    {
        private readonly ITripScheduleRepository _repository;
        private readonly IAuditService _auditService;

        public ActivateTripScheduleHandler(ITripScheduleRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(ActivateTripScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Programación de viaje no encontrada");

            if (schedule.RouteAssignment.Route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para activar la programación");

            if (await _repository.ExistsOverlapAsync(
                schedule.RouteAssignmentId,
                schedule.Direction,
                schedule.ValidFrom,
                schedule.ValidTo,
                schedule.Monday,
                schedule.Tuesday,
                schedule.Wednesday,
                schedule.Thursday,
                schedule.Friday,
                schedule.Saturday,
                schedule.Sunday,
                schedule.Id))
            {
                throw new DomainException("Ya existe una programación activa con días y vigencia solapados para esta asignación");
            }

            schedule.Activate();
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("TripScheduleActivated", "TripSchedule", schedule.Id.ToString());
            return Unit.Value;
        }
    }
}
