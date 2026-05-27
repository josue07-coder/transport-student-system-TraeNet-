using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.SystemSettings.Queries.GetSystemSettingById
{
    public class GetSystemSettingByIdHandler : IRequestHandler<GetSystemSettingByIdQuery, SystemSettingResponseDto>
    {
        private readonly ISystemSettingRepository _repository;

        public GetSystemSettingByIdHandler(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<SystemSettingResponseDto> Handle(GetSystemSettingByIdQuery request, CancellationToken cancellationToken)
        {
            var setting = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Configuración no encontrada");

            return SystemSettingMappings.ToResponseDto(setting);
        }
    }
}
