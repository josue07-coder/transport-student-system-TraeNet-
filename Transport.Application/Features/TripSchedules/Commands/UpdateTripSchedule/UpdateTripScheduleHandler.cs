using MediatR;
using System.Text.Json;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripSchedules.Commands.UpdateTripSchedule
{
    public class UpdateTripScheduleHandler : IRequestHandler<UpdateTripScheduleCommand, Unit>
    {
        private readonly ITripScheduleRepository _repository;
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IAuditService _auditService;

        public UpdateTripScheduleHandler(
            ITripScheduleRepository repository,
            IRouteAssignmentRepository assignmentRepository,
            IAuditService auditService)
        {
            _repository = repository;
            _assignmentRepository = assignmentRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(UpdateTripScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Programación de viaje no encontrada");

            var oldValues = JsonSerializer.Serialize(new
            {
                schedule.RouteAssignmentId,
                schedule.Direction,
                schedule.DepartureTime,
                schedule.ArrivalTime,
                schedule.ValidFrom,
                schedule.ValidTo,
                schedule.IsActive
            });

            var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (schedule.IsActive && assignment.Route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para mantener activa la programación");

            if (await _repository.ExistsOverlapAsync(
                request.RouteAssignmentId,
                request.Direction,
                request.ValidFrom,
                request.ValidTo,
                request.Monday,
                request.Tuesday,
                request.Wednesday,
                request.Thursday,
                request.Friday,
                request.Saturday,
                request.Sunday,
                schedule.Id))
            {
                throw new DomainException("Ya existe una programación activa con días y vigencia solapados para esta asignación");
            }

            schedule.Update(
                request.RouteAssignmentId,
                request.Direction,
                request.DepartureTime,
                request.ArrivalTime,
                request.ValidFrom,
                request.ValidTo,
                request.Monday,
                request.Tuesday,
                request.Wednesday,
                request.Thursday,
                request.Friday,
                request.Saturday,
                request.Sunday);

            await _repository.SaveChangesAsync();

            await _auditService.LogAsync(
                "TripScheduleUpdated",
                "TripSchedule",
                schedule.Id.ToString(),
                oldValues,
                JsonSerializer.Serialize(new { schedule.RouteAssignmentId, schedule.Direction, schedule.DepartureTime, schedule.ValidFrom }));

            return Unit.Value;
        }
    }
}
