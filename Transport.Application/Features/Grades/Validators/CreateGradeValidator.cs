using FluentValidation;
using Transport.Application.Features.Grades.Commands.CreateGrade;

public class CreateGradeValidator : AbstractValidator<CreateGradeCommand>
{
    public CreateGradeValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.SchoolId).NotEmpty();
    }
}