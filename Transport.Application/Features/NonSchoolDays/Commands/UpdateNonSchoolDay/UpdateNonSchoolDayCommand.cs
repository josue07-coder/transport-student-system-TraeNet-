using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.NonSchoolDays.Commands.UpdateNonSchoolDay
{
    public class UpdateNonSchoolDayCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public Guid? SchoolId { get; set; }
        public NonSchoolDayReason ReasonType { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
