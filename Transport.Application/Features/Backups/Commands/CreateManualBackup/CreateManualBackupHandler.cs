using MediatR;
using Transport.Application.Features.Backups.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Backups.Commands.CreateManualBackup
{
    public class CreateManualBackupHandler : IRequestHandler<CreateManualBackupCommand, BackupRecordResponseDto>
    {
        private readonly IBackupService _backupService;
        private readonly ICurrentUserService _currentUserService;

        public CreateManualBackupHandler(IBackupService backupService, ICurrentUserService currentUserService)
        {
            _backupService = backupService;
            _currentUserService = currentUserService;
        }

        public async Task<BackupRecordResponseDto> Handle(CreateManualBackupCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new DomainException("Usuario autenticado no encontrado");

            var backup = await _backupService.CreateManualBackupAsync(userId, cancellationToken);
            return BackupMappings.ToResponseDto(backup);
        }
    }
}
