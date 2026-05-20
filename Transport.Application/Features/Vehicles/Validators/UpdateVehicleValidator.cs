using FluentValidation;
using Transport.Application.Features.Vehicles.Commands.UpdateVehicle;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Vehicles.Validators
{
    public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.Status)
                .Must(status => status == VehicleStatus.Active || status == VehicleStatus.Inactive)
                .WithMessage("Solo se permite actualizar el vehiculo a Active o Inactive.");
        }
    }
}
