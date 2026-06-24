using Transport.Application.Features.TripRouteDeviations.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.TripRouteDeviations
{
    internal static class TripRouteDeviationMappings
    {
        public static TripRouteDeviationDto ToDto(TripRouteDeviation deviation)
        {
            return new TripRouteDeviationDto
            {
                Id = deviation.Id,
                TripId = deviation.TripId,
                ReportedByUserId = deviation.ReportedByUserId,
                ReportedByName = deviation.ReportedByUser?.Name,
                ReasonType = deviation.ReasonType,
                ReasonTypeLabel = deviation.ReasonType.ToString(),
                Reason = deviation.Reason,
                Notes = deviation.Notes,
                Latitude = deviation.Latitude,
                Longitude = deviation.Longitude,
                ReportedAt = deviation.ReportedAt,
                CreatedAt = deviation.CreatedAt
            };
        }
    }
}
