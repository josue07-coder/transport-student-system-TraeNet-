using MediatR;
using Transport.Application.Features.Drivers.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Drivers.Queries.GetDriversByActive
{
    public class GetDriversByActiveHandler : IRequestHandler<GetDriversByActiveQuery, List<DriverResponseDto>>
    {
        private readonly IDriverRepository _repository;

        public GetDriversByActiveHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DriverResponseDto>> Handle(GetDriversByActiveQuery request, CancellationToken cancellationToken)
        {
            var drivers = await _repository.GetByActiveAsync(request.IsActive);
            return drivers.Select(DriverMappings.ToResponseDto).ToList();
        }
    }
}
