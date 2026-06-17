using FluentValidation;
using Transport.Application.Features.NonSchoolDays.Commands.UpdateNonSchoolDay;

namespace Transport.Application.Features.NonSchoolDays.Validators
{
    public class UpdateNonSchoolDayValidator : AbstractValidator<UpdateNonSchoolDayCommand>
    {
        public UpdateNonSchoolDayValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.ReasonType).IsInEnum();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        }
    }
}
