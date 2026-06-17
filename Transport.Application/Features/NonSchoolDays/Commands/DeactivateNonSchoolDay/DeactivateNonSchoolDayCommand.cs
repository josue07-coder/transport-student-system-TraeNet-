using MediatR;

namespace Transport.Application.Features.NonSchoolDays.Commands.DeactivateNonSchoolDay
{
    public class DeactivateNonSchoolDayCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
