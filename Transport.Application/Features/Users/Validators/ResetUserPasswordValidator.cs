using FluentValidation;
using Transport.Application.Features.Users.Commands.ResetUserPassword;

namespace Transport.Application.Features.Users.Validators
{
    public class ResetUserPasswordValidator : AbstractValidator<ResetUserPasswordCommand>
    {
        public ResetUserPasswordValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("La confirmación de contraseña no coincide");
        }
    }
}
