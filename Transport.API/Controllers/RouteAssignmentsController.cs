using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.RouteAssignments.Commands.AssignStudentToRouteAssignment;
using Transport.Application.Features.RouteAssignments.Commands.CreateRouteAssignment;
using Transport.Application.Features.RouteAssignments.Commands.DeleteRouteAssignment;
using Transport.Application.Features.RouteAssignments.Commands.RemoveStudentFromRouteAssignment;
using Transport.Application.Features.RouteAssignments.Commands.UpdateRouteAssignment;
using Transport.Application.Features.RouteAssignments.Queries.GetAllRouteAssignments;
using Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentById;
using Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByDriver;
using Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByRoute;
using Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByVehicle;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/route-assignments")]
    public class RouteAssignmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RouteAssignmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteAssignmentCommand command)
        {
            var id = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRouteAssignmentsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRouteAssignmentByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-route/{routeId}")]
        public async Task<IActionResult> GetByRoute(Guid routeId)
        {
            var result = await _mediator.Send(new GetRouteAssignmentsByRouteQuery(routeId));
            return Ok(result);
        }

        [HttpGet("by-driver/{driverId}")]
        public async Task<IActionResult> GetByDriver(Guid driverId)
        {
            var result = await _mediator.Send(new GetRouteAssignmentsByDriverQuery(driverId));
            return Ok(result);
        }

        [HttpGet("by-vehicle/{vehicleId}")]
        public async Task<IActionResult> GetByVehicle(Guid vehicleId)
        {
            var result = await _mediator.Send(new GetRouteAssignmentsByVehicleQuery(vehicleId));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRouteAssignmentCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteRouteAssignmentCommand { Id = id });
            return NoContent();
        }

        [HttpPost("{assignmentId}/students/{studentId}")]
        public async Task<IActionResult> AssignStudent(Guid assignmentId, Guid studentId)
        {
            await _mediator.Send(new AssignStudentToRouteAssignmentCommand
            {
                RouteAssignmentId = assignmentId,
                StudentId = studentId
            });

            return NoContent();
        }

        [HttpDelete("{assignmentId}/students/{studentId}")]
        public async Task<IActionResult> RemoveStudent(Guid assignmentId, Guid studentId)
        {
            await _mediator.Send(new RemoveStudentFromRouteAssignmentCommand
            {
                RouteAssignmentId = assignmentId,
                StudentId = studentId
            });

            return NoContent();
        }
    }
}
