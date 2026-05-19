using FluentValidation;
using Transport.Application.Features.Sectors.Commands.CreateSector;

public class CreateSectorValidator : AbstractValidator<CreateSectorCommand>
{
    public CreateSectorValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Province).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
    }
}
