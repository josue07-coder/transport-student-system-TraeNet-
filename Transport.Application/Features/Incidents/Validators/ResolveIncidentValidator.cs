using FluentValidation;
using Transport.Application.Features.Incidents.Commands.ResolveIncident;

namespace Transport.Application.Features.Incidents.Validators
{
    public class ResolveIncidentValidator : AbstractValidator<ResolveIncidentCommand>
    {
        public ResolveIncidentValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
        }
    }
}
