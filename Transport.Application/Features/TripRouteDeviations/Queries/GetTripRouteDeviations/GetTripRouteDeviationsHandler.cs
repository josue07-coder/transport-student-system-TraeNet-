using MediatR;
using Transport.Application.Features.TripRouteDeviations.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripRouteDeviations.Queries.GetTripRouteDeviations
{
    public class GetTripRouteDeviationsHandler : IRequestHandler<GetTripRouteDeviationsQuery, List<TripRouteDeviationDto>>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripRouteDeviationRepository _deviationRepository;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;

        public GetTripRouteDeviationsHandler(
            ITripRepository tripRepository,
            ITripRouteDeviationRepository deviationRepository,
            ITripOperationAuthorizationService operationAuthorizationService)
        {
            _tripRepository = tripRepository;
            _deviationRepository = deviationRepository;
            _operationAuthorizationService = operationAuthorizationService;
        }

        public async Task<List<TripRouteDeviationDto>> Handle(GetTripRouteDeviationsQuery request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _operationAuthorizationService.EnsureCanReportRouteDeviationAsync(trip);

            var deviations = await _deviationRepository.GetByTripAsync(request.TripId);
            return deviations.Select(TripRouteDeviationMappings.ToDto).ToList();
        }
    }
}
