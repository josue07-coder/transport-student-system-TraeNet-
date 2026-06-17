using MediatR;
using System.Text.Json;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Features.Trips.Queries;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripSchedules.Commands.MaterializeTripSchedule
{
    public class MaterializeTripScheduleHandler : IRequestHandler<MaterializeTripScheduleCommand, TripResponseDto>
    {
        private readonly ITripScheduleRepository _scheduleRepository;
        private readonly ITripRepository _tripRepository;
        private readonly INonSchoolDayRepository _nonSchoolDayRepository;
        private readonly IAuditService _auditService;

        public MaterializeTripScheduleHandler(
            ITripScheduleRepository scheduleRepository,
            ITripRepository tripRepository,
            INonSchoolDayRepository nonSchoolDayRepository,
            IAuditService auditService)
        {
            _scheduleRepository = scheduleRepository;
            _tripRepository = tripRepository;
            _nonSchoolDayRepository = nonSchoolDayRepository;
            _auditService = auditService;
        }

        public async Task<TripResponseDto> Handle(MaterializeTripScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdWithAssignmentDetailsAsync(request.TripScheduleId)
                ?? throw new DomainException("Programación de viaje no encontrada");

            if (await _tripRepository.ExistsByScheduleAndDateAsync(schedule.Id, request.OperationDate))
                throw new DomainException("Ya existe un viaje programado para esta programación y fecha");

            var nonOperation = await GetNonOperationReasonAsync(schedule, request.OperationDate);
            ValidateSchedule(schedule, request.OperationDate, nonOperation is not null);

            var scheduledDepartureTime = request.OperationDate.ToDateTime(schedule.DepartureTime);
            var scheduledArrivalTime = schedule.ArrivalTime.HasValue
                ? request.OperationDate.ToDateTime(schedule.ArrivalTime.Value)
                : (DateTime?)null;

            var trip = Trip.CreateScheduledFromSchedule(
                schedule.RouteAssignmentId,
                schedule.Id,
                schedule.Direction,
                request.OperationDate,
                scheduledDepartureTime,
                scheduledArrivalTime);

            if (nonOperation is not null)
                trip.MarkNotOperating(nonOperation.Value.Reason, nonOperation.Value.Notes);

            await _tripRepository.AddAsync(trip);
            await _tripRepository.SaveChangesAsync();

            await _auditService.LogAsync(
                trip.Status == TripStatus.NotOperating ? "TripMaterializedAsNotOperating" : "TripMaterialized",
                "Trip",
                trip.Id.ToString(),
                null,
                JsonSerializer.Serialize(new
                {
                    trip.RouteAssignmentId,
                    trip.TripScheduleId,
                    trip.Direction,
                    trip.OperationDate,
                    trip.ScheduledDepartureTime,
                    trip.ScheduledArrivalTime,
                    trip.NonOperationReason,
                    trip.NonOperationNotes
                }));

            return TripMappings.ToResponseDto(trip);
        }

        private async Task<(string Reason, string Notes)?> GetNonOperationReasonAsync(TripSchedule schedule, DateOnly operationDate)
        {
            if (operationDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return ("Weekend", "No hay operación escolar los fines de semana.");

            var schoolId = schedule.RouteAssignment.Route.SchoolId;
            var nonSchoolDay = await _nonSchoolDayRepository.GetActiveForDateAsync(operationDate, schoolId);

            if (nonSchoolDay is null)
                return null;

            return (nonSchoolDay.ReasonType.ToString(), nonSchoolDay.Reason);
        }

        private static void ValidateSchedule(TripSchedule schedule, DateOnly operationDate, bool isNonOperating)
        {
            if (!schedule.IsActive)
                throw new DomainException("La programación de viaje está inactiva");

            if (operationDate < schedule.ValidFrom)
                throw new DomainException("La fecha de operación está fuera de la vigencia de la programación");

            if (schedule.ValidTo.HasValue && operationDate > schedule.ValidTo.Value)
                throw new DomainException("La fecha de operación está fuera de la vigencia de la programación");

            if (!isNonOperating && !IsActiveDay(schedule, operationDate.DayOfWeek))
                throw new DomainException("La fecha de operación no coincide con los días activos de la programación");

            var assignment = schedule.RouteAssignment
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (assignment.Route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para materializar el viaje");

            if (assignment.Driver is null || !assignment.Driver.IsActive)
                throw new DomainException("El conductor está inactivo");

            if (assignment.Vehicle is null || assignment.Vehicle.Status != VehicleStatus.Active)
                throw new DomainException("El vehículo está inactivo");

            if (assignment.TransportAssistant is not null && !assignment.TransportAssistant.IsActive)
                throw new DomainException("El asistente de transporte está inactivo");
        }

        private static bool IsActiveDay(TripSchedule schedule, DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => schedule.Monday,
                DayOfWeek.Tuesday => schedule.Tuesday,
                DayOfWeek.Wednesday => schedule.Wednesday,
                DayOfWeek.Thursday => schedule.Thursday,
                DayOfWeek.Friday => schedule.Friday,
                DayOfWeek.Saturday => schedule.Saturday,
                DayOfWeek.Sunday => schedule.Sunday,
                _ => false
            };
        }
    }
}
