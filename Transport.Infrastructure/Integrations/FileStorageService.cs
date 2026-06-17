using Microsoft.Extensions.Logging;
using Transport.Application.Interfaces;

namespace Transport.Infrastructure.Integrations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ISystemSettingService _settings;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(ISystemSettingService settings, ILogger<FileStorageService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var provider = await _settings.GetValueOrDefaultAsync("Storage.Provider", "Mock");
            var localPath = await _settings.GetValueOrDefaultAsync("Storage.LocalPath", "wwwroot/uploads");

            var normalizedFileName = fileName.Replace('\\', '/');
            var segments = normalizedFileName
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(Path.GetFileName)
                .OfType<string>()
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .ToArray();

            var safeRelativePath = Path.Combine(segments);
            var targetPath = Path.Combine(localPath, safeRelativePath);
            var targetDirectory = Path.GetDirectoryName(targetPath) ?? localPath;

            Directory.CreateDirectory(targetDirectory);

            await using var output = File.Create(targetPath);
            await fileStream.CopyToAsync(output, cancellationToken);

            _logger.LogInformation(
                "File upload stored locally using provider {Provider} for {FileName} with content type {ContentType}",
                provider,
                fileName,
                contentType);

            return $"/uploads/{safeRelativePath.Replace('\\', '/')}";
        }

        public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Mock/local file delete requested for {FileUrl}", fileUrl);
            return Task.CompletedTask;
        }
    }
}
