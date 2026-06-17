using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;

namespace Transport.Application.Features.TripSchedules.Queries.GetTripScheduleById
{
    public class GetTripScheduleByIdQuery : IRequest<TripScheduleDetailDto>
    {
        public Guid Id { get; }

        public GetTripScheduleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
