using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Features.RouteAssignments.Queries;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Me.Queries.GetMyRouteAssignments
{
    public class GetMyRouteAssignmentsHandler : IRequestHandler<GetMyRouteAssignmentsQuery, List<RouteAssignmentResponseDto>>
    {
        private readonly IVisibilityService _visibilityService;
        private readonly IRouteAssignmentRepository _assignmentRepository;

        public GetMyRouteAssignmentsHandler(
            IVisibilityService visibilityService,
            IRouteAssignmentRepository assignmentRepository)
        {
            _visibilityService = visibilityService;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<List<RouteAssignmentResponseDto>> Handle(GetMyRouteAssignmentsQuery request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();

            var assignments = user.DriverId.HasValue
                ? await _assignmentRepository.GetByDriverAsync(user.DriverId.Value)
                : user.TransportAssistantId.HasValue
                    ? await _assignmentRepository.GetByTransportAssistantAsync(user.TransportAssistantId.Value)
                    : user.GuardianId.HasValue
                        ? await _assignmentRepository.GetByGuardianAsync(user.GuardianId.Value)
                        : (await _assignmentRepository.GetPagedAsync(1, 100)).Items.ToList();

            assignments = await _visibilityService.FilterRouteAssignmentsAsync(assignments);

            return assignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();
        }
    }
}
