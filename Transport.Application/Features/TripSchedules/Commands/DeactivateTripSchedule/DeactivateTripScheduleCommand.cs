using MediatR;

namespace Transport.Application.Features.TripSchedules.Commands.DeactivateTripSchedule
{
    public class DeactivateTripScheduleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
