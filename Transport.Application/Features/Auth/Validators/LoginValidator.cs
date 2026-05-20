using FluentValidation;
using Transport.Application.Features.Auth.Commands.Login;

namespace Transport.Application.Features.Auth.Validators
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
