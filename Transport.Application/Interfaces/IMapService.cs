namespace Transport.Application.Interfaces
{
    public interface IMapService
    {
        Task<double> CalculateDistanceAsync(
            decimal originLat,
            decimal originLng,
            decimal destinationLat,
            decimal destinationLng,
            CancellationToken cancellationToken = default);

        Task<TimeSpan?> EstimateTravelTimeAsync(
            decimal originLat,
            decimal originLng,
            decimal destinationLat,
            decimal destinationLng,
            CancellationToken cancellationToken = default);
    }
}
