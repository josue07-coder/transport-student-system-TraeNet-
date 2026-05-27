using Transport.Application.Interfaces;

namespace Transport.Infrastructure.Integrations
{
    public class MapService : IMapService
    {
        private const double EarthRadiusKm = 6371;
        private readonly ISystemSettingService _settings;

        public MapService(ISystemSettingService settings)
        {
            _settings = settings;
        }

        public Task<double> CalculateDistanceAsync(
            decimal originLat,
            decimal originLng,
            decimal destinationLat,
            decimal destinationLng,
            CancellationToken cancellationToken = default)
        {
            var distance = CalculateHaversineKm((double)originLat, (double)originLng, (double)destinationLat, (double)destinationLng);
            return Task.FromResult(distance);
        }

        public async Task<TimeSpan?> EstimateTravelTimeAsync(
            decimal originLat,
            decimal originLng,
            decimal destinationLat,
            decimal destinationLng,
            CancellationToken cancellationToken = default)
        {
            var speed = await _settings.GetDecimalAsync("Maps.DefaultAverageSpeedKmH", 35);
            if (speed <= 0)
                return null;

            var distance = await CalculateDistanceAsync(originLat, originLng, destinationLat, destinationLng, cancellationToken);
            return TimeSpan.FromHours(distance / (double)speed);
        }

        private static double CalculateHaversineKm(double originLat, double originLng, double destinationLat, double destinationLng)
        {
            var dLat = ToRadians(destinationLat - originLat);
            var dLng = ToRadians(destinationLng - originLng);
            var lat1 = ToRadians(originLat);
            var lat2 = ToRadians(destinationLat);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat1) * Math.Cos(lat2)
                * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    }
}
