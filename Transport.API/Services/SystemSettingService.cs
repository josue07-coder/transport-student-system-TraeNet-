using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.API.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repository;

        public SystemSettingService(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GetValueAsync(string key)
        {
            var setting = await _repository.GetByKeyAsync(key);
            return setting?.Value ?? throw new DomainException($"Configuración no encontrada: {key}");
        }

        public async Task<string> GetValueOrDefaultAsync(string key, string defaultValue)
        {
            try
            {
                var setting = await _repository.GetByKeyAsync(key);
                return setting?.Value ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public async Task<int> GetIntAsync(string key, int defaultValue = 0)
        {
            var value = await GetValueOrDefaultAsync(key, defaultValue.ToString());
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public async Task<bool> GetBoolAsync(string key, bool defaultValue = false)
        {
            var value = await GetValueOrDefaultAsync(key, defaultValue.ToString());
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }

        public async Task<decimal> GetDecimalAsync(string key, decimal defaultValue = 0)
        {
            var value = await GetValueOrDefaultAsync(key, defaultValue.ToString());
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}
