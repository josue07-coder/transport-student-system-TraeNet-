using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class IncidentComment : BaseEntity
    {
        public Guid IncidentId { get; private set; }
        public Incident Incident { get; private set; } = null!;

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public string Comment { get; private set; } = string.Empty;

        private IncidentComment() { } // EF

        public IncidentComment(Guid incidentId, Guid userId, string comment)
        {
            if (incidentId == Guid.Empty)
                throw new DomainException("Incident is required");

            if (userId == Guid.Empty)
                throw new DomainException("User is required");

            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Comment is required");

            IncidentId = incidentId;
            UserId = userId;
            Comment = comment.Trim();
        }
    }
}
