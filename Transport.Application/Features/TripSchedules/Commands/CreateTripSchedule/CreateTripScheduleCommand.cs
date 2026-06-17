using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.TripSchedules.Commands.CreateTripSchedule
{
    public class CreateTripScheduleCommand : IRequest<Guid>
    {
        public Guid RouteAssignmentId { get; set; }
        public TripDirection Direction { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidTo { get; set; }
        public bool Monday { get; set; } = true;
        public bool Tuesday { get; set; } = true;
        public bool Wednesday { get; set; } = true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
