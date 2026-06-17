using FluentValidation;
using Transport.Application.Features.Users.Commands.CreateUser;

namespace Transport.Application.Features.Users.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.ProfileImageUrl).MaximumLength(300)
                .When(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl));
        }
    }
}
