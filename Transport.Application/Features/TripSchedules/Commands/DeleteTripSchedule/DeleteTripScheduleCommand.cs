using MediatR;

namespace Transport.Application.Features.TripSchedules.Commands.DeleteTripSchedule
{
    public class DeleteTripScheduleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
