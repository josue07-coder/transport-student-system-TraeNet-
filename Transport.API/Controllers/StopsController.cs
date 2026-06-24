using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Stops.Commands.CreateStop;
using Transport.Application.Features.Stops.Commands.DeleteStop;
using Transport.Application.Features.Stops.Commands.UpdateStop;
using Transport.Application.Features.Stops.Queries.GetAllStops;
using Transport.Application.Features.Stops.Queries.GetStopById;
using Transport.Application.Features.Stops.Queries.GetStopsByCity;
using Transport.Application.Features.Stops.Queries.GetStopsBySector;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/[controller]")]
    public class StopsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StopsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStopCommand command)
        {
            var id = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllStopsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetStopByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-sector/{sectorId}")]
        public async Task<IActionResult> GetBySector(Guid sectorId)
        {
            var result = await _mediator.Send(new GetStopsBySectorQuery(sectorId));
            return Ok(result);
        }

        [HttpGet("by-city/{city}")]
        public async Task<IActionResult> GetByCity(string city)
        {
            var result = await _mediator.Send(new GetStopsByCityQuery(city));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStopCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteStopCommand { Id = id });
            return NoContent();
        }
    }
}
