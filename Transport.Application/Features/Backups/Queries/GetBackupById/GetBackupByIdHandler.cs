using MediatR;
using Transport.Application.Features.Backups.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Backups.Queries.GetBackupById
{
    public class GetBackupByIdHandler : IRequestHandler<GetBackupByIdQuery, BackupRecordResponseDto>
    {
        private readonly IBackupRecordRepository _repository;

        public GetBackupByIdHandler(IBackupRecordRepository repository)
        {
            _repository = repository;
        }

        public async Task<BackupRecordResponseDto> Handle(GetBackupByIdQuery request, CancellationToken cancellationToken)
        {
            var backup = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Backup no encontrado");

            return BackupMappings.ToResponseDto(backup);
        }
    }
}
