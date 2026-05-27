using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Tracking.Commands.UpdateVehicleLocation;
using Transport.Application.Features.Tracking.Queries.GetActiveTripsLocations;
using Transport.Application.Features.Tracking.Queries.GetCurrentTripLocation;
using Transport.Application.Features.Tracking.Queries.GetMyStudentsActiveTripLocations;
using Transport.Application.Features.Tracking.Queries.GetTripLocationHistory;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/tracking")]
    public class TrackingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrackingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateVehicleLocationCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpGet("trips/{tripId:guid}/current-location")]
        public async Task<IActionResult> GetCurrentLocation(Guid tripId)
        {
            return Ok(await _mediator.Send(new GetCurrentTripLocationQuery(tripId)));
        }

        [HttpGet("trips/{tripId:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid tripId)
        {
            return Ok(await _mediator.Send(new GetTripLocationHistoryQuery(tripId)));
        }

        [HttpGet("active-trips")]
        public async Task<IActionResult> GetActiveTrips()
        {
            return Ok(await _mediator.Send(new GetActiveTripsLocationsQuery()));
        }

        [HttpGet("my-students")]
        public async Task<IActionResult> GetMyStudents()
        {
            return Ok(await _mediator.Send(new GetMyStudentsActiveTripLocationsQuery()));
        }
    }
}
