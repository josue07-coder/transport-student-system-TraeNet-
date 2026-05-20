using Transport.Application.Features.Routes.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.Routes.Queries
{
    internal static class RouteMappings
    {
        public static RouteResponseDto ToResponseDto(Route route)
        {
            return new RouteResponseDto
            {
                Id = route.Id,
                Name = route.Name,
                SchoolId = route.SchoolId,
                StartTime = route.OperatingHours.Start,
                EndTime = route.OperatingHours.End,
                Status = route.Status,
                StopCount = route.Stops.Count
            };
        }

        public static RouteDetailDto ToDetailDto(Route route)
        {
            return new RouteDetailDto
            {
                Id = route.Id,
                Name = route.Name,
                SchoolId = route.SchoolId,
                StartTime = route.OperatingHours.Start,
                EndTime = route.OperatingHours.End,
                Status = route.Status,
                Stops = route.Stops
                    .OrderBy(s => s.StopOrder)
                    .Select(s => new RouteStopDto
                    {
                        RouteStopId = s.Id,
                        StopId = s.StopId,
                        StopName = s.Stop?.Name ?? string.Empty,
                        StopOrder = s.StopOrder,
                        Street = s.Stop?.Address?.Street ?? string.Empty,
                        City = s.Stop?.Address?.City ?? string.Empty,
                        Latitude = s.Stop?.Coordinates?.Latitude ?? 0,
                        Longitude = s.Stop?.Coordinates?.Longitude ?? 0
                    })
                    .ToList()
            };
        }
    }
}
