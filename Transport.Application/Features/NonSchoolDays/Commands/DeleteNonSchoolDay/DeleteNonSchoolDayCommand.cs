using MediatR;

namespace Transport.Application.Features.NonSchoolDays.Commands.DeleteNonSchoolDay
{
    public class DeleteNonSchoolDayCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
