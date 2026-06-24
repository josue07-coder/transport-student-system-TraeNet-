using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.API.Services
{
    public class BackupService : IBackupService
    {
        private static readonly SemaphoreSlim ManualBackupLock = new(1, 1);

        private readonly AppDbContext _context;
        private readonly IBackupRecordRepository _repository;
        private readonly ISystemSettingService _settings;
        private readonly IAuditService _auditService;
        private readonly ILogger<BackupService> _logger;

        public BackupService(
            AppDbContext context,
            IBackupRecordRepository repository,
            ISystemSettingService settings,
            IAuditService auditService,
            ILogger<BackupService> logger)
        {
            _context = context;
            _repository = repository;
            _settings = settings;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<BackupRecord> CreateManualBackupAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (!await ManualBackupLock.WaitAsync(0, cancellationToken))
                throw new DomainException("Ya hay un backup en ejecución");

            try
            {
                var localPath = await _settings.GetValueOrDefaultAsync("Backup.LocalPath", "backups");
                Directory.CreateDirectory(localPath);

                var fileName = $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(localPath, fileName);
                var backupRecord = new BackupRecord(fileName, filePath, BackupType.Manual, userId);

                await _repository.AddAsync(backupRecord);
                await _repository.SaveChangesAsync();
                await _auditService.LogAsync("BackupStarted", "BackupRecord", backupRecord.Id.ToString());

                try
                {
                    backupRecord.MarkInProgress();
                    await _repository.SaveChangesAsync();

                    var payload = await CreateBackupPayloadAsync(userId, cancellationToken);
                    var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });

                    await File.WriteAllTextAsync(filePath, json, cancellationToken);
                    var fileInfo = new FileInfo(filePath);

                    backupRecord.MarkCompleted(fileInfo.Length);
                    await _repository.SaveChangesAsync();
                    await _auditService.LogAsync("BackupCompleted", "BackupRecord", backupRecord.Id.ToString(), null, JsonSerializer.Serialize(new { backupRecord.FileName, backupRecord.FileSizeBytes }));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Manual backup failed");
                    backupRecord.MarkFailed(ex.Message);
                    await _repository.SaveChangesAsync();
                    await _auditService.LogAsync("BackupFailed", "BackupRecord", backupRecord.Id.ToString(), null, ex.Message);
                }

                return backupRecord;
            }
            finally
            {
                ManualBackupLock.Release();
            }
        }

        public Task<BackupRecord?> GetLatestBackupAsync(CancellationToken cancellationToken)
        {
            return _repository.GetLatestAsync();
        }

        private async Task<object> CreateBackupPayloadAsync(Guid userId, CancellationToken cancellationToken)
        {
            return new
            {
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                DatabaseProvider = _context.Database.ProviderName,
                Counts = new
                {
                    Students = await _context.Students.CountAsync(cancellationToken),
                    Guardians = await _context.Guardians.CountAsync(cancellationToken),
                    Schools = await _context.Schools.CountAsync(cancellationToken),
                    Vehicles = await _context.Vehicles.CountAsync(cancellationToken),
                    Drivers = await _context.Drivers.CountAsync(cancellationToken),
                    TransportAssistants = await _context.TransportAssistants.CountAsync(cancellationToken),
                    Routes = await _context.Routes.CountAsync(cancellationToken),
                    RouteAssignments = await _context.RouteAssignments.CountAsync(cancellationToken),
                    Trips = await _context.Trips.CountAsync(cancellationToken),
                    Users = await _context.Users.CountAsync(cancellationToken),
                    Incidents = await _context.Incidents.CountAsync(cancellationToken),
                    Notifications = await _context.Notifications.CountAsync(cancellationToken)
                }
            };
        }
    }
}
