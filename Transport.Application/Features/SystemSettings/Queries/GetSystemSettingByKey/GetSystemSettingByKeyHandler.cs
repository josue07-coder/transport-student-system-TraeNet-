using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.SystemSettings.Queries.GetSystemSettingByKey
{
    public class GetSystemSettingByKeyHandler : IRequestHandler<GetSystemSettingByKeyQuery, SystemSettingResponseDto>
    {
        private readonly ISystemSettingRepository _repository;

        public GetSystemSettingByKeyHandler(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<SystemSettingResponseDto> Handle(GetSystemSettingByKeyQuery request, CancellationToken cancellationToken)
        {
            var setting = await _repository.GetByKeyAsync(request.Key)
                ?? throw new DomainException("Configuración no encontrada");

            return SystemSettingMappings.ToResponseDto(setting);
        }
    }
}
