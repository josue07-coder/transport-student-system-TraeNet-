using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Routes.Commands.AddStopToRoute;
using Transport.Application.Features.Routes.Commands.CreateRoute;
using Transport.Application.Features.Routes.Commands.DeleteRoute;
using Transport.Application.Features.Routes.Commands.RemoveStopFromRoute;
using Transport.Application.Features.Routes.Commands.UpdateRoute;
using Transport.Application.Features.Routes.Commands.UpdateRouteStopOrder;
using Transport.Application.Features.Routes.Queries.GetAllRoutes;
using Transport.Application.Features.Routes.Queries.GetRouteById;
using Transport.Application.Features.Routes.Queries.GetRoutesBySchool;
using Transport.Application.Features.Routes.Queries.GetRoutesByStatus;
using Transport.Domain.Enums;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoutesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRoutesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRouteByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-school/{schoolId}")]
        public async Task<IActionResult> GetBySchool(Guid schoolId)
        {
            var result = await _mediator.Send(new GetRoutesBySchoolQuery(schoolId));
            return Ok(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(RouteStatus status)
        {
            var result = await _mediator.Send(new GetRoutesByStatusQuery(status));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRouteCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteRouteCommand { Id = id });
            return NoContent();
        }

        [HttpPost("{routeId}/stops")]
        public async Task<IActionResult> AddStop(Guid routeId, [FromBody] AddStopToRouteCommand command)
        {
            if (routeId != command.RouteId)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{routeId}/stops/{stopId}")]
        public async Task<IActionResult> RemoveStop(Guid routeId, Guid stopId)
        {
            await _mediator.Send(new RemoveStopFromRouteCommand
            {
                RouteId = routeId,
                StopId = stopId
            });

            return NoContent();
        }

        [HttpPut("{routeId}/stops/{stopId}/order")]
        public async Task<IActionResult> UpdateStopOrder(Guid routeId, Guid stopId, [FromBody] UpdateRouteStopOrderCommand command)
        {
            if (routeId != command.RouteId || stopId != command.StopId)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
