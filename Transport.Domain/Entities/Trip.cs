using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Trip : BaseEntity
    {
        public Guid RouteAssignmentId { get; private set; }
        public RouteAssignment RouteAssignment { get; private set; } = null!;

        public Guid? TripScheduleId { get; private set; }
        public TripSchedule? TripSchedule { get; private set; }
        public TripDirection? Direction { get; private set; }
        public DateOnly? OperationDate { get; private set; }
        public DateTime? ScheduledDepartureTime { get; private set; }
        public DateTime? ScheduledArrivalTime { get; private set; }

        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        public TripStatus Status { get; private set; }
        public string? CancellationReason { get; private set; }
        public string? NonOperationReason { get; private set; }
        public string? NonOperationNotes { get; private set; }

        private readonly List<TripStudentAttendance> _studentAttendances = new();
        public IReadOnlyCollection<TripStudentAttendance> StudentAttendances => _studentAttendances.AsReadOnly();

        public bool IsActive => Status == TripStatus.InProgress;

        private Trip() { } // EF Core

        public Trip(Guid routeAssignmentId)
        {
            if (routeAssignmentId == Guid.Empty)
                throw new DomainException("Route assignment is required");

            RouteAssignmentId = routeAssignmentId;
            Status = TripStatus.Scheduled;
        }

        public Trip(
            Guid routeAssignmentId,
            Guid tripScheduleId,
            TripDirection direction,
            DateOnly operationDate,
            DateTime scheduledDepartureTime,
            DateTime? scheduledArrivalTime = null)
            : this(routeAssignmentId)
        {
            SetSchedule(tripScheduleId, direction, operationDate, scheduledDepartureTime, scheduledArrivalTime);
        }

        public static Trip CreateScheduledFromSchedule(
            Guid routeAssignmentId,
            Guid tripScheduleId,
            TripDirection direction,
            DateOnly operationDate,
            DateTime scheduledDepartureTime,
            DateTime? scheduledArrivalTime = null)
        {
            return new Trip(routeAssignmentId, tripScheduleId, direction, operationDate, scheduledDepartureTime, scheduledArrivalTime);
        }

        public void SetSchedule(
            Guid tripScheduleId,
            TripDirection direction,
            DateOnly operationDate,
            DateTime scheduledDepartureTime,
            DateTime? scheduledArrivalTime = null)
        {
            if (tripScheduleId == Guid.Empty)
                throw new DomainException("La programación del viaje es obligatoria");

            if (!Enum.IsDefined(typeof(TripDirection), direction))
                throw new DomainException("La dirección del viaje no es válida");

            if (operationDate == default)
                throw new DomainException("La fecha de operación es obligatoria");

            if (scheduledDepartureTime == default)
                throw new DomainException("La hora programada de salida es obligatoria");

            if (scheduledArrivalTime.HasValue && scheduledArrivalTime.Value <= scheduledDepartureTime)
                throw new DomainException("La hora programada de llegada debe ser mayor que la salida");

            TripScheduleId = tripScheduleId;
            Direction = direction;
            OperationDate = operationDate;
            ScheduledDepartureTime = scheduledDepartureTime;
            ScheduledArrivalTime = scheduledArrivalTime;
        }

        public void Start()
        {
            if (Status != TripStatus.Scheduled)
                throw new DomainException("Solo se puede iniciar un viaje programado");

            StartTime = DateTime.UtcNow;
            Status = TripStatus.InProgress;
        }

        public void End()
        {
            if (Status != TripStatus.InProgress)
                throw new DomainException("Trip is not in progress");

            EndTime = DateTime.UtcNow;
            Status = TripStatus.Completed;
        }

        public void Cancel(string reason)
        {
            if (Status != TripStatus.Scheduled)
                throw new DomainException("Solo se puede cancelar un viaje programado");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("La razón de cancelación es obligatoria");

            CancellationReason = reason.Trim();
            Status = TripStatus.Cancelled;
        }

        public void MarkNotOperating(string reason, string? notes)
        {
            if (Status != TripStatus.Scheduled)
                throw new DomainException("Solo se puede marcar como no operativo un viaje programado");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("La razón de no operación es obligatoria");

            NonOperationReason = reason.Trim();
            NonOperationNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Status = TripStatus.NotOperating;
        }
    }
}
