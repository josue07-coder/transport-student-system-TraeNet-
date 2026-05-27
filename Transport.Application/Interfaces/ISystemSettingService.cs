namespace Transport.Application.Interfaces
{
    public interface ISystemSettingService
    {
        Task<string> GetValueAsync(string key);
        Task<string> GetValueOrDefaultAsync(string key, string defaultValue);
        Task<int> GetIntAsync(string key, int defaultValue = 0);
        Task<bool> GetBoolAsync(string key, bool defaultValue = false);
        Task<decimal> GetDecimalAsync(string key, decimal defaultValue = 0);
    }
}
