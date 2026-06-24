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
        public int DelayMinutes { get; private set; }
        public bool IsLate { get; private set; }
        public bool StartedEarly { get; private set; }
        public string? EarlyStartReason { get; private set; }
        public TripPunctualityStatus PunctualityStatus { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        private readonly List<TripStudentAttendance> _studentAttendances = new();
        public IReadOnlyCollection<TripStudentAttendance> StudentAttendances => _studentAttendances.AsReadOnly();

        private readonly List<TripRouteDeviation> _routeDeviations = new();
        public IReadOnlyCollection<TripRouteDeviation> RouteDeviations => _routeDeviations.AsReadOnly();

        public bool IsActive => Status == TripStatus.InProgress;

        private Trip() { } // EF Core

        public Trip(Guid routeAssignmentId)
        {
            if (routeAssignmentId == Guid.Empty)
                throw new DomainException("Route assignment is required");

            RouteAssignmentId = routeAssignmentId;
            Status = TripStatus.Scheduled;
            PunctualityStatus = TripPunctualityStatus.OnTime;
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
            Start(DateTime.UtcNow, 0, false, null);
        }

        public void Start(DateTime currentTime, int toleranceMinutes, bool forceEarlyStart, string? earlyStartReason)
        {
            if (Status != TripStatus.Scheduled)
                throw new DomainException("Solo se puede iniciar un viaje programado");

            if (ScheduledDepartureTime.HasValue)
            {
                var earliestStartTime = ScheduledDepartureTime.Value.AddMinutes(-Math.Max(toleranceMinutes, 0));
                if (currentTime < earliestStartTime)
                {
                    if (!forceEarlyStart)
                        throw new DomainException("No puede iniciar el viaje antes de la hora programada.");

                    if (string.IsNullOrWhiteSpace(earlyStartReason))
                        throw new DomainException("La razón de inicio anticipado es obligatoria");

                    StartedEarly = true;
                    EarlyStartReason = earlyStartReason.Trim();
                }

                DelayMinutes = Math.Max(0, (int)Math.Floor((currentTime - ScheduledDepartureTime.Value).TotalMinutes));
                IsLate = DelayMinutes > 0;
                PunctualityStatus = DelayMinutes <= 0
                    ? TripPunctualityStatus.OnTime
                    : DelayMinutes <= 10
                        ? TripPunctualityStatus.SlightlyLate
                        : TripPunctualityStatus.Late;
            }
            else
            {
                DelayMinutes = 0;
                IsLate = false;
                PunctualityStatus = TripPunctualityStatus.OnTime;
            }

            StartTime = currentTime;
            Status = TripStatus.InProgress;
        }

        public void End()
        {
            if (Status != TripStatus.InProgress)
                throw new DomainException("El viaje no está en progreso");

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

        public TripRouteDeviation ReportRouteDeviation(
            Guid reportedByUserId,
            RouteDeviationReasonType reasonType,
            string? reason,
            string? notes,
            decimal? latitude,
            decimal? longitude,
            DateTime reportedAt)
        {
            if (Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede reportar un desvío en un viaje en progreso");

            var deviation = new TripRouteDeviation(
                Id,
                reportedByUserId,
                reasonType,
                reason,
                notes,
                latitude,
                longitude,
                reportedAt);

            _routeDeviations.Add(deviation);
            return deviation;
        }

        public TripStudentAttendance AddExceptionalPassenger(
            Guid studentId,
            string studentNameSnapshot,
            string studentCodeSnapshot,
            Guid? guardianIdSnapshot,
            string? guardianNameSnapshot,
            string exceptionReason,
            Guid registeredByUserId,
            DateTime boardedAt,
            string? notes)
        {
            if (Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede agregar un pasajero excepcional en un viaje en progreso");

            if (_studentAttendances.Any(attendance => attendance.StudentId == studentId))
                throw new DomainException("El estudiante ya está registrado como pasajero de este viaje");

            var attendance = TripStudentAttendance.CreateExceptionalPassenger(
                Id,
                studentId,
                studentNameSnapshot,
                studentCodeSnapshot,
                guardianIdSnapshot,
                guardianNameSnapshot,
                exceptionReason,
                registeredByUserId,
                boardedAt,
                notes);

            _studentAttendances.Add(attendance);
            return attendance;
        }
    }
}
