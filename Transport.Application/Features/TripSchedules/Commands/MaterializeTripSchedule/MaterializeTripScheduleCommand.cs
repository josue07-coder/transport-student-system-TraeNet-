using MediatR;
using Transport.Application.Features.Trips.DTOs;

namespace Transport.Application.Features.TripSchedules.Commands.MaterializeTripSchedule
{
    public class MaterializeTripScheduleCommand : IRequest<TripResponseDto>
    {
        public Guid TripScheduleId { get; set; }
        public DateOnly OperationDate { get; set; }
    }
}
