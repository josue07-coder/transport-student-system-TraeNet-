using MediatR;
using System.Text.Json;
using Transport.Application.Features.SystemSettings.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.SystemSettings.Commands.DeleteSystemSetting
{
    public class DeleteSystemSettingHandler : IRequestHandler<DeleteSystemSettingCommand>
    {
        private readonly ISystemSettingRepository _repository;
        private readonly IAuditService _auditService;

        public DeleteSystemSettingHandler(ISystemSettingRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task Handle(DeleteSystemSettingCommand request, CancellationToken cancellationToken)
        {
            var setting = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Configuración no encontrada");

            if (!setting.IsEditable)
                throw new DomainException("No se puede eliminar una configuración base no editable");

            var oldValues = JsonSerializer.Serialize(SystemSettingMappings.ToResponseDto(setting));
            _repository.Remove(setting);
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("SystemSettingDeleted", "SystemSetting", setting.Id.ToString(), oldValues, null);
        }
    }
}
