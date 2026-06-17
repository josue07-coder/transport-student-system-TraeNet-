using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.TripSchedules.Commands.ActivateTripSchedule;
using Transport.Application.Features.TripSchedules.Commands.CreateTripSchedule;
using Transport.Application.Features.TripSchedules.Commands.DeactivateTripSchedule;
using Transport.Application.Features.TripSchedules.Commands.DeleteTripSchedule;
using Transport.Application.Features.TripSchedules.Commands.MaterializeTripSchedule;
using Transport.Application.Features.TripSchedules.Commands.UpdateTripSchedule;
using Transport.Application.Features.TripSchedules.Queries.GetActiveTripSchedulesByAssignment;
using Transport.Application.Features.TripSchedules.Queries.GetAllTripSchedules;
using Transport.Application.Features.TripSchedules.Queries.GetTripScheduleById;
using Transport.Application.Features.TripSchedules.Queries.GetTripSchedulesByAssignment;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/trip-schedules")]
    public class TripSchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripSchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllTripSchedulesQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetTripScheduleByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-assignment/{routeAssignmentId}")]
        public async Task<IActionResult> GetByAssignment(Guid routeAssignmentId)
        {
            var result = await _mediator.Send(new GetTripSchedulesByAssignmentQuery(routeAssignmentId));
            return Ok(result);
        }

        [HttpGet("by-assignment/{routeAssignmentId}/active")]
        public async Task<IActionResult> GetActiveByAssignment(Guid routeAssignmentId)
        {
            var result = await _mediator.Send(new GetActiveTripSchedulesByAssignmentQuery(routeAssignmentId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTripScheduleCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpPost("{id}/materialize")]
        public async Task<IActionResult> Materialize(Guid id, [FromBody] MaterializeTripScheduleCommand command)
        {
            if (id != command.TripScheduleId)
                return BadRequest();

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTripScheduleCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivateTripScheduleCommand { Id = id });
            return NoContent();
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateTripScheduleCommand { Id = id });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteTripScheduleCommand { Id = id });
            return NoContent();
        }
    }
}
