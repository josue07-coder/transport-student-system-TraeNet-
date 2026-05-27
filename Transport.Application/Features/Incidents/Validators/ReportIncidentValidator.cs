using FluentValidation;
using Transport.Application.Features.Incidents.Commands.ReportIncident;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Incidents.Validators
{
    public class ReportIncidentValidator : AbstractValidator<ReportIncidentCommand>
    {
        public ReportIncidentValidator()
        {
            RuleFor(command => command.Title).NotEmpty().MaximumLength(200);
            RuleFor(command => command.Description).NotEmpty().MaximumLength(2000);
            RuleFor(command => command.Type).IsInEnum();
            RuleFor(command => command.Severity).IsInEnum();
        }
    }
}
