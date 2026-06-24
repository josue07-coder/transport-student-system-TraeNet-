using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class TripStudentAttendance
    {
        public Guid TripId { get; private set; }
        public Guid StudentId { get; private set; }
        public string StudentNameSnapshot { get; private set; } = string.Empty;
        public string StudentCodeSnapshot { get; private set; } = string.Empty;
        public Guid? GuardianIdSnapshot { get; private set; }
        public string? GuardianNameSnapshot { get; private set; }
        public TripAttendanceStatus Status { get; private set; }
        public DateTime? BoardedAt { get; private set; }
        public DateTime? DroppedOffAt { get; private set; }
        public string? Notes { get; private set; }
        public bool IsExpectedPassenger { get; private set; }
        public string? ExceptionReason { get; private set; }
        public Guid? RegisteredByUserId { get; private set; }
        public User? RegisteredByUser { get; private set; }
        public DateTime? RegisteredAt { get; private set; }
        public Guid? MarkedByUserId { get; private set; }
        public User? MarkedByUser { get; private set; }
        public DateTime? MarkedAt { get; private set; }
        public DateTime? AbsenceNotifiedAt { get; private set; }
        public DateTime? BoardedNotificationSentAt { get; private set; }
        public AttendanceSource AttendanceSource { get; private set; } = AttendanceSource.Manual;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        public Trip Trip { get; private set; } = null!;
        public Student Student { get; private set; } = null!;

        private TripStudentAttendance() { }

        public TripStudentAttendance(
            Guid tripId,
            Guid studentId,
            string studentNameSnapshot,
            string studentCodeSnapshot,
            Guid? guardianIdSnapshot,
            string? guardianNameSnapshot)
        {
            if (tripId == Guid.Empty)
                throw new DomainException("El viaje es obligatorio");

            if (studentId == Guid.Empty)
                throw new DomainException("El estudiante es obligatorio");

            if (string.IsNullOrWhiteSpace(studentNameSnapshot))
                throw new DomainException("El nombre del estudiante es obligatorio");

            if (string.IsNullOrWhiteSpace(studentCodeSnapshot))
                throw new DomainException("El codigo del estudiante es obligatorio");

            TripId = tripId;
            StudentId = studentId;
            StudentNameSnapshot = studentNameSnapshot.Trim();
            StudentCodeSnapshot = studentCodeSnapshot.Trim();
            GuardianIdSnapshot = guardianIdSnapshot;
            GuardianNameSnapshot = string.IsNullOrWhiteSpace(guardianNameSnapshot) ? null : guardianNameSnapshot.Trim();
            Status = TripAttendanceStatus.Expected;
            IsExpectedPassenger = true;
            AttendanceSource = AttendanceSource.Manual;
            CreatedAt = DateTime.UtcNow;
        }

        public static TripStudentAttendance CreateExceptionalPassenger(
            Guid tripId,
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
            if (registeredByUserId == Guid.Empty)
                throw new DomainException("El usuario que registra el pasajero excepcional es obligatorio");

            if (string.IsNullOrWhiteSpace(exceptionReason))
                throw new DomainException("La razon del pasajero excepcional es obligatoria");

            if (boardedAt == default)
                throw new DomainException("La hora de abordaje del pasajero excepcional es obligatoria");

            var attendance = new TripStudentAttendance(
                tripId,
                studentId,
                studentNameSnapshot,
                studentCodeSnapshot,
                guardianIdSnapshot,
                guardianNameSnapshot);

            attendance.IsExpectedPassenger = false;
            attendance.ExceptionReason = exceptionReason.Trim();
            attendance.RegisteredByUserId = registeredByUserId;
            attendance.RegisteredAt = DateTime.UtcNow;
            attendance.BoardedAt = boardedAt;
            attendance.MarkedByUserId = registeredByUserId;
            attendance.MarkedAt = boardedAt;
            attendance.AttendanceSource = AttendanceSource.ExceptionalManual;
            attendance.Status = TripAttendanceStatus.Boarded;
            attendance.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            return attendance;
        }

        public void MarkBoarded(DateTime boardedAt)
        {
            MarkBoarded(boardedAt, null);
        }

        public void MarkBoarded(DateTime boardedAt, Guid? markedByUserId)
        {
            if (Status == TripAttendanceStatus.DroppedOff)
                throw new DomainException("El estudiante ya fue marcado como entregado");

            if (Status == TripAttendanceStatus.Absent)
                throw new DomainException("No se puede abordar un estudiante marcado ausente");

            BoardedAt = boardedAt;
            MarkedAt = boardedAt;
            MarkedByUserId = markedByUserId;
            AttendanceSource = IsExpectedPassenger ? AttendanceSource.Manual : AttendanceSource.ExceptionalManual;
            Status = TripAttendanceStatus.Boarded;
            SetUpdated();
        }

        public void MarkAbsent(string? notes)
        {
            MarkAbsent(notes, null, DateTime.UtcNow);
        }

        public void MarkAbsent(string? notes, Guid? markedByUserId, DateTime markedAt)
        {
            if (Status == TripAttendanceStatus.Boarded)
                throw new DomainException("No se puede marcar ausente un estudiante que ya abordó");

            if (Status == TripAttendanceStatus.DroppedOff)
                throw new DomainException("No se puede marcar ausente un estudiante ya entregado");

            Status = TripAttendanceStatus.Absent;
            MarkedAt = markedAt;
            MarkedByUserId = markedByUserId;
            AttendanceSource = IsExpectedPassenger ? AttendanceSource.Manual : AttendanceSource.ExceptionalManual;
            Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
            SetUpdated();
        }

        public void MarkDroppedOff(DateTime droppedOffAt)
        {
            MarkDroppedOff(droppedOffAt, null);
        }

        public void MarkDroppedOff(DateTime droppedOffAt, Guid? markedByUserId)
        {
            if (Status != TripAttendanceStatus.Boarded)
                throw new DomainException("Solo se puede entregar un estudiante abordado");

            DroppedOffAt = droppedOffAt;
            MarkedAt = droppedOffAt;
            MarkedByUserId = markedByUserId;
            Status = TripAttendanceStatus.DroppedOff;
            SetUpdated();
        }

        public void UpdateNotes(string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                throw new DomainException("La nota es obligatoria");

            Notes = notes.Trim();
            SetUpdated();
        }

        public void MarkBoardedNotificationSent(DateTime notifiedAt)
        {
            BoardedNotificationSentAt = notifiedAt;
            SetUpdated();
        }

        public void MarkAbsenceNotificationSent(DateTime notifiedAt)
        {
            AbsenceNotifiedAt = notifiedAt;
            SetUpdated();
        }

        private void SetUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
