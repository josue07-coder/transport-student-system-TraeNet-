using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;

namespace Transport.Application.Features.TripSchedules.Queries.GetAllTripSchedules
{
    public class GetAllTripSchedulesQuery : IRequest<List<TripScheduleResponseDto>>
    {
    }
}
