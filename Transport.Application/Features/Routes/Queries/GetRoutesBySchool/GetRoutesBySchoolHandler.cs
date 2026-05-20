using MediatR;
using Transport.Application.Features.Routes.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Routes.Queries.GetRoutesBySchool
{
    public class GetRoutesBySchoolHandler : IRequestHandler<GetRoutesBySchoolQuery, List<RouteResponseDto>>
    {
        private readonly IRouteRepository _repository;

        public GetRoutesBySchoolHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RouteResponseDto>> Handle(GetRoutesBySchoolQuery request, CancellationToken cancellationToken)
        {
            var routes = await _repository.GetBySchoolAsync(request.SchoolId);
            return routes.Select(RouteMappings.ToResponseDto).ToList();
        }
    }
}
