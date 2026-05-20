using FluentValidation;
using Transport.Application.Features.Vehicles.Commands.CreateVehicle;

namespace Transport.Application.Features.Vehicles.Validators
{
    public class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleValidator()
        {
            RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Capacity).GreaterThan(0);
        }
    }
}
