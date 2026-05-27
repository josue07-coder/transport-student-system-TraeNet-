using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.SystemSettings.Queries.GetSystemSettingsByCategory
{
    public class GetSystemSettingsByCategoryHandler : IRequestHandler<GetSystemSettingsByCategoryQuery, List<SystemSettingResponseDto>>
    {
        private readonly ISystemSettingRepository _repository;

        public GetSystemSettingsByCategoryHandler(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SystemSettingResponseDto>> Handle(GetSystemSettingsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var settings = await _repository.GetByCategoryAsync(request.Category);
            return settings.Select(SystemSettingMappings.ToResponseDto).ToList();
        }
    }
}
