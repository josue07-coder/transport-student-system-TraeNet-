using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Drivers.Commands.CreateDriver;
using Transport.Application.Features.Drivers.Commands.DeleteDriver;
using Transport.Application.Features.Drivers.Commands.UpdateDriver;
using Transport.Application.Features.Drivers.Queries.GetAllDrivers;
using Transport.Application.Features.Drivers.Queries.GetDriverById;
using Transport.Application.Features.Drivers.Queries.GetDriverByLicenseNumber;
using Transport.Application.Features.Drivers.Queries.GetDriversByActive;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DriversController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDriverCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDriversQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDriverByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("license/{licenseNumber}")]
        public async Task<IActionResult> GetByLicenseNumber(string licenseNumber)
        {
            var result = await _mediator.Send(new GetDriverByLicenseNumberQuery(licenseNumber));
            return Ok(result);
        }

        [HttpGet("by-active/{isActive}")]
        public async Task<IActionResult> GetByActive(bool isActive)
        {
            var result = await _mediator.Send(new GetDriversByActiveQuery(isActive));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDriverCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteDriverCommand { Id = id });
            return NoContent();
        }
    }
}
