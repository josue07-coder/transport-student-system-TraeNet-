using MediatR;
using System.Text.Json;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripSchedules.Commands.CreateTripSchedule
{
    public class CreateTripScheduleHandler : IRequestHandler<CreateTripScheduleCommand, Guid>
    {
        private readonly ITripScheduleRepository _repository;
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IAuditService _auditService;

        public CreateTripScheduleHandler(
            ITripScheduleRepository repository,
            IRouteAssignmentRepository assignmentRepository,
            IAuditService auditService)
        {
            _repository = repository;
            _assignmentRepository = assignmentRepository;
            _auditService = auditService;
        }

        public async Task<Guid> Handle(CreateTripScheduleCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (assignment.Route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para crear una programación activa");

            if (await HasOverlapAsync(request))
                throw new DomainException("Ya existe una programación activa con días y vigencia solapados para esta asignación");

            var schedule = new TripSchedule(
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

            await _repository.AddAsync(schedule);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync(
                "TripScheduleCreated",
                "TripSchedule",
                schedule.Id.ToString(),
                null,
                JsonSerializer.Serialize(new { schedule.RouteAssignmentId, schedule.Direction, schedule.DepartureTime, schedule.ValidFrom }));

            return schedule.Id;
        }

        private Task<bool> HasOverlapAsync(CreateTripScheduleCommand request)
        {
            return _repository.ExistsOverlapAsync(
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
                request.Sunday);
        }
    }
}
