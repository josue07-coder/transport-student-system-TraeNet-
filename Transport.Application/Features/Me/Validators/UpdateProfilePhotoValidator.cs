using FluentValidation;
using Transport.Application.Features.Me.Commands.UpdateProfilePhoto;

namespace Transport.Application.Features.Me.Validators
{
    public class UpdateProfilePhotoValidator : AbstractValidator<UpdateProfilePhotoCommand>
    {
        public UpdateProfilePhotoValidator()
        {
            RuleFor(x => x.ProfileImageUrl)
                .NotEmpty()
                .MaximumLength(300);
        }
    }
}
