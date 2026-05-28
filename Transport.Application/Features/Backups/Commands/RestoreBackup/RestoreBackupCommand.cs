using MediatR;

namespace Transport.Application.Features.Backups.Commands.RestoreBackup
{
    public record RestoreBackupCommand(Guid Id) : IRequest;
}
