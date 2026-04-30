using FluentValidation;

namespace Transport.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Student code is required");

            RuleFor(x => x.SchoolId)
                .NotEmpty().WithMessage("School is required");
        }
    }
}