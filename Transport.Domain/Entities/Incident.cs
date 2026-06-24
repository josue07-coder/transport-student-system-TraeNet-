using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Incident : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public IncidentType Type { get; private set; }
        public IncidentSeverity Severity { get; private set; }
        public IncidentStatus Status { get; private set; }

        public Guid? TripId { get; private set; }
        public Trip? Trip { get; private set; }

        public Guid? RouteAssignmentId { get; private set; }
        public RouteAssignment? RouteAssignment { get; private set; }

        public Guid? VehicleId { get; private set; }
        public Vehicle? Vehicle { get; private set; }

        public Guid? DriverId { get; private set; }
        public Driver? Driver { get; private set; }

        public Guid? TransportAssistantId { get; private set; }
        public TransportAssistant? TransportAssistant { get; private set; }

        public Guid ReportedByUserId { get; private set; }
        public User ReportedByUser { get; private set; } = null!;

        public Guid? AssignedToUserId { get; private set; }
        public User? AssignedToUser { get; private set; }

        public Guid? ResolvedByUserId { get; private set; }
        public User? ResolvedByUser { get; private set; }

        public DateTime? ResolvedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        private readonly List<IncidentComment> _comments = new();
        public IReadOnlyCollection<IncidentComment> Comments => _comments.AsReadOnly();

        private Incident() { } // EF

        public Incident(
            string title,
            string description,
            IncidentType type,
            IncidentSeverity severity,
            Guid reportedByUserId,
            Guid? tripId = null,
            Guid? routeAssignmentId = null,
            Guid? vehicleId = null,
            Guid? driverId = null,
            Guid? transportAssistantId = null)
        {
            if (reportedByUserId == Guid.Empty)
                throw new DomainException("Reported user is required");

            SetContent(title, description);
            Type = type;
            Severity = severity;
            Status = IncidentStatus.Open;
            ReportedByUserId = reportedByUserId;
            TripId = tripId;
            RouteAssignmentId = routeAssignmentId;
            VehicleId = vehicleId;
            DriverId = driverId;
            TransportAssistantId = transportAssistantId;
        }

        public void AssignTo(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("Assigned user is required");

            EnsureNotClosedOrCancelled("No se puede asignar un incidente cerrado o cancelado");
            AssignedToUserId = userId;
            SetUpdated();
        }

        public void MarkInProgress()
        {
            EnsureNotClosedOrCancelled("No se puede cambiar un incidente cerrado o cancelado");
            Status = IncidentStatus.InProgress;
            SetUpdated();
        }

        public void Resolve(Guid resolvedByUserId)
        {
            if (resolvedByUserId == Guid.Empty)
                throw new DomainException("Resolved user is required");

            EnsureNotClosedOrCancelled("No se puede resolver un incidente cerrado o cancelado");
            Status = IncidentStatus.Resolved;
            ResolvedByUserId = resolvedByUserId;
            ResolvedAt = DateTime.UtcNow;
            SetUpdated();
        }

        public void Close()
        {
            if (Status != IncidentStatus.Resolved)
                throw new DomainException("No se puede cerrar un incidente que no está resuelto");

            Status = IncidentStatus.Closed;
            ClosedAt = DateTime.UtcNow;
            SetUpdated();
        }

        public void Cancel()
        {
            if (Status == IncidentStatus.Closed)
                throw new DomainException("No se puede cancelar un incidente cerrado");

            if (Status == IncidentStatus.Cancelled)
                throw new DomainException("El incidente ya está cancelado");

            Status = IncidentStatus.Cancelled;
            SetUpdated();
        }

        public IncidentComment AddComment(Guid userId, string comment)
        {
            EnsureNotClosedOrCancelled("No se puede comentar un incidente cerrado o cancelado");
            var incidentComment = new IncidentComment(Id, userId, comment);
            _comments.Add(incidentComment);
            SetUpdated();
            return incidentComment;
        }

        private void SetContent(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title is required");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");

            Title = title.Trim();
            Description = description.Trim();
        }

        private void EnsureNotClosedOrCancelled(string message)
        {
            if (Status is IncidentStatus.Closed or IncidentStatus.Cancelled)
                throw new DomainException(message);
        }
    }
}
