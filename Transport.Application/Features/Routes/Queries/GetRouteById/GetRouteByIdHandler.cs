using MediatR;
using Transport.Application.Features.Routes.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Routes.Queries.GetRouteById
{
    public class GetRouteByIdHandler : IRequestHandler<GetRouteByIdQuery, RouteDetailDto>
    {
        private readonly IRouteRepository _repository;

        public GetRouteByIdHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<RouteDetailDto> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdWithStopsAsync(request.Id)
                ?? throw new DomainException("Ruta no encontrada");

            return RouteMappings.ToDetailDto(route);
        }
    }
}
