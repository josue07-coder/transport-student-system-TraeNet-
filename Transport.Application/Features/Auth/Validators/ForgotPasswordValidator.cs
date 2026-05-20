using FluentValidation;
using Transport.Application.Features.Auth.Commands.ForgotPassword;

namespace Transport.Application.Features.Auth.Validators
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.DocumentNumber).NotEmpty();
            RuleFor(x => x.PhoneOrEmail).NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("La confirmación de contraseña no coincide");
        }
    }
}
