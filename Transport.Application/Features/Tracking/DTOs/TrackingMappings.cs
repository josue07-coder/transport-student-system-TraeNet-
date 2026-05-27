using Transport.Domain.Entities;

namespace Transport.Application.Features.Tracking.DTOs
{
    public static class TrackingMappings
    {
        public static VehicleLocationResponseDto ToResponseDto(this VehicleLocation location)
        {
            return new VehicleLocationResponseDto
            {
                Id = location.Id,
                TripId = location.TripId,
                VehicleId = location.VehicleId,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Speed = location.Speed,
                Heading = location.Heading,
                RecordedAt = location.RecordedAt
            };
        }

        public static CurrentTripLocationDto ToCurrentDto(this VehicleLocation location)
        {
            var assignment = location.Trip.RouteAssignment;

            return new CurrentTripLocationDto
            {
                TripId = location.TripId,
                VehicleId = location.VehicleId,
                RouteName = assignment.Route?.Name,
                DriverName = assignment.Driver == null ? null : $"{assignment.Driver.FirstName} {assignment.Driver.LastName}",
                TransportAssistantName = assignment.TransportAssistant == null ? null : $"{assignment.TransportAssistant.FirstName} {assignment.TransportAssistant.LastName}",
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Speed = location.Speed,
                Heading = location.Heading,
                RecordedAt = location.RecordedAt
            };
        }
    }
}
