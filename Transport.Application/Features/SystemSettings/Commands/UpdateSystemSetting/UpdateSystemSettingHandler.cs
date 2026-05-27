using MediatR;
using System.Text.Json;
using Transport.Application.Features.SystemSettings.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.SystemSettings.Commands.UpdateSystemSetting
{
    public class UpdateSystemSettingHandler : IRequestHandler<UpdateSystemSettingCommand, SystemSettingResponseDto>
    {
        private readonly ISystemSettingRepository _repository;
        private readonly IAuditService _auditService;

        public UpdateSystemSettingHandler(ISystemSettingRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<SystemSettingResponseDto> Handle(UpdateSystemSettingCommand request, CancellationToken cancellationToken)
        {
            var setting = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Configuración no encontrada");

            var oldValues = JsonSerializer.Serialize(SystemSettingMappings.ToResponseDto(setting));
            setting.Update(request.Value, request.Category, request.DataType, request.Description, request.IsEditable);

            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("SystemSettingUpdated", "SystemSetting", setting.Id.ToString(), oldValues, JsonSerializer.Serialize(SystemSettingMappings.ToResponseDto(setting)));

            return SystemSettingMappings.ToResponseDto(setting);
        }
    }
}
