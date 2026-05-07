using FluentValidation;
using Transport.Application.Features.Schools.Commands.CreateSchool;

public class CreateSchoolValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.DirectorName).NotEmpty();

        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty();

        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();

        RuleFor(x => x.SectorId).NotEmpty();
    }
}