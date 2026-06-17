using MediatR;

namespace Transport.Application.Features.TripSchedules.Commands.ActivateTripSchedule
{
    public class ActivateTripScheduleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
