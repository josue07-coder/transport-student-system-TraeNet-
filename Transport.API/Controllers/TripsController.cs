using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Trips.Commands.CancelTrip;
using Transport.Application.Features.Trips.Commands.EndTrip;
using Transport.Application.Features.Trips.Commands.MarkTripNotOperating;
using Transport.Application.Features.Trips.Commands.StartTrip;
using Transport.Application.Features.Trips.Queries.GetActiveTripByRouteAssignment;
using Transport.Application.Features.Trips.Queries.GetAllTrips;
using Transport.Application.Features.Trips.Queries.GetTripById;
using Transport.Application.Features.Trips.Queries.GetTripsByDateRange;
using Transport.Application.Features.Trips.Queries.GetTripsByRouteAssignment;
using Transport.Application.Features.Trips.Queries.GetTripsByStatus;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentAbsent;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentBoarded;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentDroppedOff;
using Transport.Application.Features.TripStudentAttendances.Commands.UpdateTripStudentNotes;
using Transport.Application.Features.TripStudentAttendances.Queries.GetTripPassengers;
using Transport.Domain.Enums;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/trips")]
    public class TripsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartTripCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpPut("{id}/end")]
        public async Task<IActionResult> End(Guid id)
        {
            await _mediator.Send(new EndTripCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelTripCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("{id}/not-operating")]
        public async Task<IActionResult> MarkNotOperating(Guid id, [FromBody] MarkTripNotOperatingCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTripsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetTripByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("{tripId}/passengers")]
        public async Task<IActionResult> GetPassengers(Guid tripId)
        {
            var result = await _mediator.Send(new GetTripPassengersQuery(tripId));
            return Ok(result);
        }

        [HttpPut("{tripId}/students/{studentId}/boarded")]
        public async Task<IActionResult> MarkStudentBoarded(Guid tripId, Guid studentId)
        {
            await _mediator.Send(new MarkStudentBoardedCommand
            {
                TripId = tripId,
                StudentId = studentId
            });

            return NoContent();
        }

        [HttpPut("{tripId}/students/{studentId}/absent")]
        public async Task<IActionResult> MarkStudentAbsent(Guid tripId, Guid studentId, [FromBody] MarkStudentAbsentCommand command)
        {
            command.TripId = tripId;
            command.StudentId = studentId;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("{tripId}/students/{studentId}/dropped-off")]
        public async Task<IActionResult> MarkStudentDroppedOff(Guid tripId, Guid studentId)
        {
            await _mediator.Send(new MarkStudentDroppedOffCommand
            {
                TripId = tripId,
                StudentId = studentId
            });

            return NoContent();
        }

        [HttpPut("{tripId}/students/{studentId}/notes")]
        public async Task<IActionResult> UpdateStudentNotes(Guid tripId, Guid studentId, [FromBody] UpdateTripStudentNotesCommand command)
        {
            command.TripId = tripId;
            command.StudentId = studentId;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("by-route-assignment/{routeAssignmentId}")]
        public async Task<IActionResult> GetByRouteAssignment(Guid routeAssignmentId)
        {
            var result = await _mediator.Send(new GetTripsByRouteAssignmentQuery(routeAssignmentId));
            return Ok(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(TripStatus status)
        {
            var result = await _mediator.Send(new GetTripsByStatusQuery(status));
            return Ok(result);
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetTripsByDateRangeQuery(startDate, endDate));
            return Ok(result);
        }

        [HttpGet("active/by-route-assignment/{routeAssignmentId}")]
        public async Task<IActionResult> GetActiveByRouteAssignment(Guid routeAssignmentId)
        {
            var result = await _mediator.Send(new GetActiveTripByRouteAssignmentQuery(routeAssignmentId));
            return Ok(result);
        }
    }
}
