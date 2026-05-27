using FluentValidation;
using Transport.Application.Features.Incidents.Commands.AddIncidentComment;

namespace Transport.Application.Features.Incidents.Validators
{
    public class AddIncidentCommentValidator : AbstractValidator<AddIncidentCommentCommand>
    {
        public AddIncidentCommentValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.Comment).NotEmpty().MaximumLength(1000);
        }
    }
}
