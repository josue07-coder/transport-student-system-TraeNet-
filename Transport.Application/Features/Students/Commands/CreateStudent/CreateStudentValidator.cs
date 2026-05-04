using FluentValidation;

namespace Transport.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Apellidos obligatorios");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El codigo del estudiante es obligatorio");

            RuleFor(x => x.SchoolId)
                .NotEmpty().WithMessage("La escuela es obligatoria");
        }
    }
}