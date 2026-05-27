using MediatR;
using System.Text.Json;
using Transport.Application.Features.SystemSettings.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.SystemSettings.Commands.CreateSystemSetting
{
    public class CreateSystemSettingHandler : IRequestHandler<CreateSystemSettingCommand, SystemSettingResponseDto>
    {
        private readonly ISystemSettingRepository _repository;
        private readonly IAuditService _auditService;

        public CreateSystemSettingHandler(ISystemSettingRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<SystemSettingResponseDto> Handle(CreateSystemSettingCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByKeyAsync(request.Key))
                throw new DomainException("Ya existe una configuración con esa clave");

            var setting = new SystemSetting(
                request.Key,
                request.Value,
                request.Category,
                request.DataType,
                request.Description,
                request.IsEditable);

            await _repository.AddAsync(setting);
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("SystemSettingCreated", "SystemSetting", setting.Id.ToString(), null, JsonSerializer.Serialize(SystemSettingMappings.ToResponseDto(setting)));

            return SystemSettingMappings.ToResponseDto(setting);
        }
    }
}
