using FluentValidation;
using Transport.Application.Features.RouteAssignments.Commands.AssignStudentToRouteAssignment;

namespace Transport.Application.Features.RouteAssignments.Validators
{
    public class AssignStudentToRouteAssignmentValidator : AbstractValidator<AssignStudentToRouteAssignmentCommand>
    {
        public AssignStudentToRouteAssignmentValidator()
        {
            RuleFor(x => x.RouteAssignmentId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
        }
    }
}
