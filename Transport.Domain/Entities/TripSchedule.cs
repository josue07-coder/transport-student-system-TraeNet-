using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class TripSchedule : BaseEntity
    {
        public Guid RouteAssignmentId { get; private set; }
        public TripDirection Direction { get; private set; }
        public TimeOnly DepartureTime { get; private set; }
        public TimeOnly? ArrivalTime { get; private set; }
        public bool IsActive { get; private set; }
        public DateOnly ValidFrom { get; private set; }
        public DateOnly? ValidTo { get; private set; }
        public bool Monday { get; private set; }
        public bool Tuesday { get; private set; }
        public bool Wednesday { get; private set; }
        public bool Thursday { get; private set; }
        public bool Friday { get; private set; }
        public bool Saturday { get; private set; }
        public bool Sunday { get; private set; }

        public RouteAssignment RouteAssignment { get; private set; } = null!;

        private readonly List<Trip> _trips = new();
        public IReadOnlyCollection<Trip> Trips => _trips.AsReadOnly();

        private TripSchedule() { }

        public TripSchedule(
            Guid routeAssignmentId,
            TripDirection direction,
            TimeOnly departureTime,
            TimeOnly? arrivalTime,
            DateOnly validFrom,
            DateOnly? validTo = null,
            bool monday = true,
            bool tuesday = true,
            bool wednesday = true,
            bool thursday = true,
            bool friday = true,
            bool saturday = false,
            bool sunday = false)
        {
            Update(
                routeAssignmentId,
                direction,
                departureTime,
                arrivalTime,
                validFrom,
                validTo,
                monday,
                tuesday,
                wednesday,
                thursday,
                friday,
                saturday,
                sunday);

            IsActive = true;
        }

        public void Update(
            Guid routeAssignmentId,
            TripDirection direction,
            TimeOnly departureTime,
            TimeOnly? arrivalTime,
            DateOnly validFrom,
            DateOnly? validTo,
            bool monday,
            bool tuesday,
            bool wednesday,
            bool thursday,
            bool friday,
            bool saturday,
            bool sunday)
        {
            if (routeAssignmentId == Guid.Empty)
                throw new DomainException("La asignación de ruta es obligatoria");

            if (!Enum.IsDefined(typeof(TripDirection), direction))
                throw new DomainException("La dirección del viaje no es válida");

            if (arrivalTime.HasValue && arrivalTime.Value <= departureTime)
                throw new DomainException("La hora de llegada debe ser mayor que la hora de salida");

            if (validTo.HasValue && validTo.Value < validFrom)
                throw new DomainException("La fecha final no puede ser menor que la fecha inicial");

            if (!HasAnyActiveDay(monday, tuesday, wednesday, thursday, friday, saturday, sunday))
                throw new DomainException("La programación debe tener al menos un día activo");

            RouteAssignmentId = routeAssignmentId;
            Direction = direction;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Monday = monday;
            Tuesday = tuesday;
            Wednesday = wednesday;
            Thursday = thursday;
            Friday = friday;
            Saturday = saturday;
            Sunday = sunday;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static bool HasAnyActiveDay(
            bool monday,
            bool tuesday,
            bool wednesday,
            bool thursday,
            bool friday,
            bool saturday,
            bool sunday)
        {
            return monday || tuesday || wednesday || thursday || friday || saturday || sunday;
        }
    }
}
