using MediatR;

namespace Transport.Application.Features.Incidents.Commands.AddIncidentComment
{
    public class AddIncidentCommentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
