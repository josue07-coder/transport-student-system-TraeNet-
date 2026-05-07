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

            RuleFor(x => x.SchoolId)
                .NotEmpty().WithMessage("La escuela es obligatoria");

            RuleFor(x => x.GradeId)
                .NotEmpty().WithMessage("El grado es obligatorio");
        }
    }
}