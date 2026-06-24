using MediatR;
using Transport.Application.Features.TripRouteDeviations.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripRouteDeviations.Queries.GetTripRouteDeviationById
{
    public class GetTripRouteDeviationByIdHandler : IRequestHandler<GetTripRouteDeviationByIdQuery, TripRouteDeviationDto>
    {
        private readonly ITripRouteDeviationRepository _deviationRepository;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;

        public GetTripRouteDeviationByIdHandler(
            ITripRouteDeviationRepository deviationRepository,
            ITripOperationAuthorizationService operationAuthorizationService)
        {
            _deviationRepository = deviationRepository;
            _operationAuthorizationService = operationAuthorizationService;
        }

        public async Task<TripRouteDeviationDto> Handle(GetTripRouteDeviationByIdQuery request, CancellationToken cancellationToken)
        {
            var deviation = await _deviationRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Desvío de ruta no encontrado");

            await _operationAuthorizationService.EnsureCanReportRouteDeviationAsync(deviation.Trip);

            return TripRouteDeviationMappings.ToDto(deviation);
        }
    }
}
