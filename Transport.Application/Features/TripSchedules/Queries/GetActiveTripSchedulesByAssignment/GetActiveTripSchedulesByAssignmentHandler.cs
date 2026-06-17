using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.TripSchedules.Queries.GetActiveTripSchedulesByAssignment
{
    public class GetActiveTripSchedulesByAssignmentHandler : IRequestHandler<GetActiveTripSchedulesByAssignmentQuery, List<TripScheduleResponseDto>>
    {
        private readonly ITripScheduleRepository _repository;

        public GetActiveTripSchedulesByAssignmentHandler(ITripScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TripScheduleResponseDto>> Handle(GetActiveTripSchedulesByAssignmentQuery request, CancellationToken cancellationToken)
        {
            var schedules = await _repository.GetActiveByAssignmentAsync(request.RouteAssignmentId);
            return schedules.Select(TripScheduleMappings.ToResponseDto).ToList();
        }
    }
}
