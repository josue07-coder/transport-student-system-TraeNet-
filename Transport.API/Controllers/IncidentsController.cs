using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Incidents.Commands.AddIncidentComment;
using Transport.Application.Features.Incidents.Commands.AssignIncident;
using Transport.Application.Features.Incidents.Commands.CancelIncident;
using Transport.Application.Features.Incidents.Commands.CloseIncident;
using Transport.Application.Features.Incidents.Commands.MarkIncidentInProgress;
using Transport.Application.Features.Incidents.Commands.ReportIncident;
using Transport.Application.Features.Incidents.Commands.ResolveIncident;
using Transport.Application.Features.Incidents.Queries.GetAllIncidents;
using Transport.Application.Features.Incidents.Queries.GetIncidentById;
using Transport.Application.Features.Incidents.Queries.GetIncidentsByRouteAssignment;
using Transport.Application.Features.Incidents.Queries.GetIncidentsBySeverity;
using Transport.Application.Features.Incidents.Queries.GetIncidentsByStatus;
using Transport.Application.Features.Incidents.Queries.GetIncidentsByTrip;
using Transport.Application.Features.Incidents.Queries.GetMyReportedIncidents;
using Transport.Domain.Enums;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/incidents")]
    public class IncidentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IncidentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Report([FromBody] ReportIncidentCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllIncidentsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(new GetIncidentByIdQuery(id)));
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(IncidentStatus status)
        {
            return Ok(await _mediator.Send(new GetIncidentsByStatusQuery(status)));
        }

        [HttpGet("by-severity/{severity}")]
        public async Task<IActionResult> GetBySeverity(IncidentSeverity severity)
        {
            return Ok(await _mediator.Send(new GetIncidentsBySeverityQuery(severity)));
        }

        [HttpGet("by-trip/{tripId:guid}")]
        public async Task<IActionResult> GetByTrip(Guid tripId)
        {
            return Ok(await _mediator.Send(new GetIncidentsByTripQuery(tripId)));
        }

        [HttpGet("by-route-assignment/{routeAssignmentId:guid}")]
        public async Task<IActionResult> GetByRouteAssignment(Guid routeAssignmentId)
        {
            return Ok(await _mediator.Send(new GetIncidentsByRouteAssignmentQuery(routeAssignmentId)));
        }

        [HttpGet("my-reported")]
        public async Task<IActionResult> GetMyReported()
        {
            return Ok(await _mediator.Send(new GetMyReportedIncidentsQuery()));
        }

        [HttpPut("{id:guid}/assign/{userId:guid}")]
        public async Task<IActionResult> Assign(Guid id, Guid userId)
        {
            await _mediator.Send(new AssignIncidentCommand { Id = id, UserId = userId });
            return NoContent();
        }

        [HttpPut("{id:guid}/in-progress")]
        public async Task<IActionResult> MarkInProgress(Guid id)
        {
            await _mediator.Send(new MarkIncidentInProgressCommand { Id = id });
            return NoContent();
        }

        [HttpPut("{id:guid}/resolve")]
        public async Task<IActionResult> Resolve(Guid id)
        {
            await _mediator.Send(new ResolveIncidentCommand { Id = id });
            return NoContent();
        }

        [HttpPut("{id:guid}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            await _mediator.Send(new CloseIncidentCommand { Id = id });
            return NoContent();
        }

        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _mediator.Send(new CancelIncidentCommand { Id = id });
            return NoContent();
        }

        [HttpPost("{id:guid}/comments")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] AddIncidentCommentCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
