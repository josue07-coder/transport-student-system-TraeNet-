using FluentValidation;
using Transport.Application.Features.RouteAssignments.Commands.CreateRouteAssignment;

namespace Transport.Application.Features.RouteAssignments.Validators
{
    public class CreateRouteAssignmentValidator : AbstractValidator<CreateRouteAssignmentCommand>
    {
        public CreateRouteAssignmentValidator()
        {
            RuleFor(x => x.RouteId).NotEmpty();
            RuleFor(x => x.DriverId).NotEmpty();
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.VehicleCapacity).GreaterThan(0);
        }
    }
}
