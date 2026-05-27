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
            if (!string.Equals(provider, "Local", StringComparison.OrdinalIgnoreCase))
            {
                var mockUrl = $"/uploads/mock/{Guid.NewGuid()}-{Path.GetFileName(fileName)}";
                _logger.LogInformation("Mock file upload created for {FileName} with content type {ContentType}", fileName, contentType);
                return mockUrl;
            }

            var localPath = await _settings.GetValueOrDefaultAsync("Storage.LocalPath", "wwwroot/uploads");
            Directory.CreateDirectory(localPath);

            var safeFileName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";
            var targetPath = Path.Combine(localPath, safeFileName);

            await using var output = File.Create(targetPath);
            await fileStream.CopyToAsync(output, cancellationToken);

            return $"/uploads/{safeFileName}";
        }

        public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Mock/local file delete requested for {FileUrl}", fileUrl);
            return Task.CompletedTask;
        }
    }
}
