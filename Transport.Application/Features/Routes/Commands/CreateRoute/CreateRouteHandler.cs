using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Routes.Commands.CreateRoute
{
    public class CreateRouteHandler : IRequestHandler<CreateRouteCommand, Guid>
    {
        private readonly IRouteRepository _repository;

        public CreateRouteHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = new Route(
                request.Name,
                request.SchoolId,
                new TimeRange(request.StartTime, request.EndTime));

            await _repository.AddAsync(route);
            await _repository.SaveChangesAsync();

            return route.Id;
        }
    }
}
