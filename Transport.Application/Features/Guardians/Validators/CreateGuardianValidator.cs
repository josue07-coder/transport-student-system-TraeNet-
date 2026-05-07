using FluentValidation;
using Transport.Application.Features.Guardians.Commands.CreateGuardian;

public class CreateGuardianValidator : AbstractValidator<CreateGuardianCommand>
{
    public CreateGuardianValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.SectorId).NotEmpty();
    }
}