using FluentValidation;
using Transport.Application.Features.Me.Commands.UpdateMeProfile;

namespace Transport.Application.Features.Me.Validators
{
    public class UpdateMeProfileValidator : AbstractValidator<UpdateMeProfileCommand>
    {
        public UpdateMeProfileValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.ProfileImageUrl)
                .MaximumLength(300)
                .When(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl));
        }
    }
}
