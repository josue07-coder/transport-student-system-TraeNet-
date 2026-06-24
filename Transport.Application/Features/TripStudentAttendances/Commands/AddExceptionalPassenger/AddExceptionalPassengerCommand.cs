using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;

namespace Transport.Application.Features.TripStudentAttendances.Commands.AddExceptionalPassenger
{
    public class AddExceptionalPassengerCommand : IRequest<TripStudentAttendanceDto>
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string ExceptionReason { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? BoardedAt { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
