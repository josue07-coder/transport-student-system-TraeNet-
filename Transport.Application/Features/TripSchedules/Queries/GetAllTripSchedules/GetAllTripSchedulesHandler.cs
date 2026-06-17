using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.TripSchedules.Queries.GetAllTripSchedules
{
    public class GetAllTripSchedulesHandler : IRequestHandler<GetAllTripSchedulesQuery, List<TripScheduleResponseDto>>
    {
        private readonly ITripScheduleRepository _repository;

        public GetAllTripSchedulesHandler(ITripScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TripScheduleResponseDto>> Handle(GetAllTripSchedulesQuery request, CancellationToken cancellationToken)
        {
            var schedules = await _repository.GetAllAsync();
            return schedules.Select(TripScheduleMappings.ToResponseDto).ToList();
        }
    }
}
