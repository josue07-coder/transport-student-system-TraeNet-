using FluentValidation;
using Transport.Application.Features.Incidents.Commands.AssignIncident;

namespace Transport.Application.Features.Incidents.Validators
{
    public class AssignIncidentValidator : AbstractValidator<AssignIncidentCommand>
    {
        public AssignIncidentValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.UserId).NotEmpty();
        }
    }
}
