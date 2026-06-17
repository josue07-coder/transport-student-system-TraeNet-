using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.TripSchedules.Queries.GetTripSchedulesByAssignment
{
    public class GetTripSchedulesByAssignmentHandler : IRequestHandler<GetTripSchedulesByAssignmentQuery, List<TripScheduleResponseDto>>
    {
        private readonly ITripScheduleRepository _repository;

        public GetTripSchedulesByAssignmentHandler(ITripScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TripScheduleResponseDto>> Handle(GetTripSchedulesByAssignmentQuery request, CancellationToken cancellationToken)
        {
            var schedules = await _repository.GetByAssignmentAsync(request.RouteAssignmentId);
            return schedules.Select(TripScheduleMappings.ToResponseDto).ToList();
        }
    }
}
