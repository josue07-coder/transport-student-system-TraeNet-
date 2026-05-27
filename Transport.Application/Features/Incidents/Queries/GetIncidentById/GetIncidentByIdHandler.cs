using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentById
{
    public class GetIncidentByIdHandler : IRequestHandler<GetIncidentByIdQuery, IncidentDetailDto>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetIncidentByIdHandler(IIncidentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<IncidentDetailDto> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
        {
            var incident = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Incidente no encontrado");

            await IncidentAccess.EnsureCanViewAsync(incident, _visibilityService);
            return incident.ToDetailDto();
        }
    }
}
