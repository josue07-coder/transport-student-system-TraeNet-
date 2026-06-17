using FluentValidation;
using Transport.Application.Features.NonSchoolDays.Commands.CreateNonSchoolDay;

namespace Transport.Application.Features.NonSchoolDays.Validators
{
    public class CreateNonSchoolDayValidator : AbstractValidator<CreateNonSchoolDayCommand>
    {
        public CreateNonSchoolDayValidator()
        {
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.ReasonType).IsInEnum();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        }
    }
}
