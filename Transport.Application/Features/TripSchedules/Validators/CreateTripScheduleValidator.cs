using FluentValidation;
using Transport.Application.Features.TripSchedules.Commands.CreateTripSchedule;
using Transport.Domain.Enums;

namespace Transport.Application.Features.TripSchedules.Validators
{
    public class CreateTripScheduleValidator : AbstractValidator<CreateTripScheduleCommand>
    {
        public CreateTripScheduleValidator()
        {
            RuleFor(x => x.RouteAssignmentId).NotEmpty();
            RuleFor(x => x.Direction).IsInEnum();
            RuleFor(x => x.ValidFrom).NotEmpty();
            RuleFor(x => x.ArrivalTime)
                .GreaterThan(x => x.DepartureTime)
                .When(x => x.ArrivalTime.HasValue);
            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidTo.HasValue);
            RuleFor(x => x)
                .Must(HasAnyActiveDay)
                .WithMessage("La programación debe tener al menos un día activo");
        }

        private static bool HasAnyActiveDay(CreateTripScheduleCommand command)
        {
            return command.Monday || command.Tuesday || command.Wednesday || command.Thursday ||
                   command.Friday || command.Saturday || command.Sunday;
        }
    }
}
