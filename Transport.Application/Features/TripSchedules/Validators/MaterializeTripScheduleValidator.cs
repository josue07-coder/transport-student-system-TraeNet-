using FluentValidation;
using Transport.Application.Features.TripSchedules.Commands.MaterializeTripSchedule;

namespace Transport.Application.Features.TripSchedules.Validators
{
    public class MaterializeTripScheduleValidator : AbstractValidator<MaterializeTripScheduleCommand>
    {
        public MaterializeTripScheduleValidator()
        {
            RuleFor(x => x.TripScheduleId).NotEmpty();
            RuleFor(x => x.OperationDate).NotEmpty();
        }
    }
}
