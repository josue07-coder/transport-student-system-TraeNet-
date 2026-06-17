using FluentValidation;
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
