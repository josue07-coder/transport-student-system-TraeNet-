using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class TripRouteDeviation : BaseEntity
    {
        public Guid TripId { get; private set; }
        public Trip Trip { get; private set; } = null!;

        public Guid ReportedByUserId { get; private set; }
        public User ReportedByUser { get; private set; } = null!;

        public RouteDeviationReasonType ReasonType { get; private set; }
        public string? Reason { get; private set; }
        public string? Notes { get; private set; }
        public decimal? Latitude { get; private set; }
        public decimal? Longitude { get; private set; }
        public DateTime ReportedAt { get; private set; }

        private TripRouteDeviation() { }

        public TripRouteDeviation(
            Guid tripId,
            Guid reportedByUserId,
            RouteDeviationReasonType reasonType,
            string? reason,
            string? notes,
            decimal? latitude,
            decimal? longitude,
            DateTime reportedAt)
        {
            if (tripId == Guid.Empty)
                throw new DomainException("El viaje es obligatorio");

            if (reportedByUserId == Guid.Empty)
                throw new DomainException("El usuario que reporta es obligatorio");

            if (!Enum.IsDefined(typeof(RouteDeviationReasonType), reasonType))
                throw new DomainException("El tipo de desvío no es válido");

            if (reasonType == RouteDeviationReasonType.Other && string.IsNullOrWhiteSpace(reason))
                throw new DomainException("La razón del desvío es obligatoria cuando el tipo es Other");

            if (!string.IsNullOrWhiteSpace(reason) && reason.Trim().Length > 300)
                throw new DomainException("La razón del desvío no puede exceder 300 caracteres");

            if (!string.IsNullOrWhiteSpace(notes) && notes.Trim().Length > 1000)
                throw new DomainException("Las notas del desvío no pueden exceder 1000 caracteres");

            TripId = tripId;
            ReportedByUserId = reportedByUserId;
            ReasonType = reasonType;
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Latitude = latitude;
            Longitude = longitude;
            ReportedAt = reportedAt == default ? DateTime.UtcNow : reportedAt;
        }
    }
}
