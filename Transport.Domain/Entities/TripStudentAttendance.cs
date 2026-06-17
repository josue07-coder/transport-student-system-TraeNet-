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
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

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
                throw new DomainException("El código del estudiante es obligatorio");

            TripId = tripId;
            StudentId = studentId;
            StudentNameSnapshot = studentNameSnapshot.Trim();
            StudentCodeSnapshot = studentCodeSnapshot.Trim();
            GuardianIdSnapshot = guardianIdSnapshot;
            GuardianNameSnapshot = string.IsNullOrWhiteSpace(guardianNameSnapshot) ? null : guardianNameSnapshot.Trim();
            Status = TripAttendanceStatus.Expected;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkBoarded(DateTime boardedAt)
        {
            if (Status == TripAttendanceStatus.DroppedOff)
                throw new DomainException("El estudiante ya fue marcado como entregado");

            if (Status == TripAttendanceStatus.Absent)
                throw new DomainException("No se puede abordar un estudiante marcado ausente");

            BoardedAt = boardedAt;
            Status = TripAttendanceStatus.Boarded;
            SetUpdated();
        }

        public void MarkAbsent(string? notes)
        {
            if (Status == TripAttendanceStatus.DroppedOff)
                throw new DomainException("No se puede marcar ausente un estudiante ya entregado");

            Status = TripAttendanceStatus.Absent;
            Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
            SetUpdated();
        }

        public void MarkDroppedOff(DateTime droppedOffAt)
        {
            if (Status != TripAttendanceStatus.Boarded)
                throw new DomainException("Solo se puede entregar un estudiante abordado");

            DroppedOffAt = droppedOffAt;
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

        private void SetUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
