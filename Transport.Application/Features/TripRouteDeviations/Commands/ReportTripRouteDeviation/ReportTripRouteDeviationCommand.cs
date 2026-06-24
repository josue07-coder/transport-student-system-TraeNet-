using MediatR;
using System.Text.Json.Serialization;
using Transport.Application.Features.TripRouteDeviations.DTOs;
using Transport.Domain.Enums;

namespace Transport.Application.Features.TripRouteDeviations.Commands.ReportTripRouteDeviation
{
    public class ReportTripRouteDeviationCommand : IRequest<TripRouteDeviationDto>
    {
        public Guid TripId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RouteDeviationReasonType ReasonType { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
