using FluentValidation;
using Transport.Application.Features.TripStudentAttendances.Commands.AddExceptionalPassenger;
using Transport.Application.Features.TripStudentAttendances.Commands.CloseTripAttendance;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkAttendanceBoarded;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentAbsent;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentBoarded;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentDroppedOff;
using Transport.Application.Features.TripStudentAttendances.Commands.UpdateTripStudentNotes;

namespace Transport.Application.Features.TripStudentAttendances.Validators
{
    public class MarkStudentBoardedValidator : AbstractValidator<MarkStudentBoardedCommand>
    {
        public MarkStudentBoardedValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
        }
    }

    public class MarkAttendanceBoardedValidator : AbstractValidator<MarkAttendanceBoardedCommand>
    {
        public MarkAttendanceBoardedValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.ExceptionReason).MaximumLength(500);
        }
    }

    public class CloseTripAttendanceValidator : AbstractValidator<CloseTripAttendanceCommand>
    {
        public CloseTripAttendanceValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();
        }
    }

    public class AddExceptionalPassengerValidator : AbstractValidator<AddExceptionalPassengerCommand>
    {
        public AddExceptionalPassengerValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.ExceptionReason).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Notes).MaximumLength(1000);
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90m, 90m)
                .When(x => x.Latitude.HasValue);
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180m, 180m)
                .When(x => x.Longitude.HasValue);
        }
    }

    public class MarkStudentAbsentValidator : AbstractValidator<MarkStudentAbsentCommand>
    {
        public MarkStudentAbsentValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(500);
        }
    }

    public class MarkStudentDroppedOffValidator : AbstractValidator<MarkStudentDroppedOffCommand>
    {
        public MarkStudentDroppedOffValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
        }
    }

    public class UpdateTripStudentNotesValidator : AbstractValidator<UpdateTripStudentNotesCommand>
    {
        public UpdateTripStudentNotesValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Notes).NotEmpty().MaximumLength(500);
        }
    }
}
