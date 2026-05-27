using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Commands.AddIncidentComment
{
    public class AddIncidentCommentHandler : IRequestHandler<AddIncidentCommentCommand, Unit>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;

        public AddIncidentCommentHandler(IIncidentRepository repository, IVisibilityService visibilityService, INotificationService notificationService, IAuditService auditService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
            _notificationService = notificationService;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(AddIncidentCommentCommand request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            var incident = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Incidente no encontrado");

            await IncidentAccess.EnsureCanViewAsync(incident, _visibilityService);
            var comment = incident.AddComment(user.Id, request.Comment);
            await _repository.AddCommentAsync(comment);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync("IncidentCommentAdded", "Incident", incident.Id.ToString(), null, $"{{\"CommentId\":\"{comment.Id}\"}}");
            await IncidentNotificationHelper.NotifyCommentAddedAsync(incident, user.Id, _notificationService);

            return Unit.Value;
        }
    }
}
