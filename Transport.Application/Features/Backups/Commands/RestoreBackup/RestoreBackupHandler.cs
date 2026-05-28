using MediatR;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Backups.Commands.RestoreBackup
{
    public class RestoreBackupHandler : IRequestHandler<RestoreBackupCommand>
    {
        public Task Handle(RestoreBackupCommand request, CancellationToken cancellationToken)
        {
            throw new DomainException("Restore is not implemented in this phase");
        }
    }
}
