using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.SystemSettings.Queries.GetAllSystemSettings
{
    public class GetAllSystemSettingsHandler : IRequestHandler<GetAllSystemSettingsQuery, List<SystemSettingResponseDto>>
    {
        private readonly ISystemSettingRepository _repository;

        public GetAllSystemSettingsHandler(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SystemSettingResponseDto>> Handle(GetAllSystemSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _repository.GetAllAsync();
            return settings.Select(SystemSettingMappings.ToResponseDto).ToList();
        }
    }
}
