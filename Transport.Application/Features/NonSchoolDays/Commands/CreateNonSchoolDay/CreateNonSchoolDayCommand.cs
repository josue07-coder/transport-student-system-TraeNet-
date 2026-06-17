using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.NonSchoolDays.Commands.CreateNonSchoolDay
{
    public class CreateNonSchoolDayCommand : IRequest<Guid>
    {
        public DateOnly Date { get; set; }
        public Guid? SchoolId { get; set; }
        public NonSchoolDayReason ReasonType { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
