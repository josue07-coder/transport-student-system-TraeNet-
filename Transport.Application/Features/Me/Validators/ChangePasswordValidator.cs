using FluentValidation;
using Transport.Application.Features.Me.Commands.ChangePassword;

namespace Transport.Application.Features.Me.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("La confirmación de contraseña no coincide");
        }
    }
}
