using FluentValidation;
using Transport.Application.Features.RouteAssignments.Commands.UpdateRouteAssignment;

namespace Transport.Application.Features.RouteAssignments.Validators
{
    public class UpdateRouteAssignmentValidator : AbstractValidator<UpdateRouteAssignmentCommand>
    {
        public UpdateRouteAssignmentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.RouteId).NotEmpty();
            RuleFor(x => x.DriverId).NotEmpty();
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.VehicleCapacity).GreaterThan(0);
        }
    }
}
