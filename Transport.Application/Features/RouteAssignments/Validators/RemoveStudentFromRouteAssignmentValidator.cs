using FluentValidation;
using Transport.Application.Features.RouteAssignments.Commands.RemoveStudentFromRouteAssignment;

namespace Transport.Application.Features.RouteAssignments.Validators
{
    public class RemoveStudentFromRouteAssignmentValidator : AbstractValidator<RemoveStudentFromRouteAssignmentCommand>
    {
        public RemoveStudentFromRouteAssignmentValidator()
        {
            RuleFor(x => x.RouteAssignmentId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
        }
    }
}
