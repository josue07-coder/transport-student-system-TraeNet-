using MediatR;

namespace Transport.Application.Features.SystemSettings.Commands.DeleteSystemSetting
{
    public record DeleteSystemSettingCommand(Guid Id) : IRequest;
}
