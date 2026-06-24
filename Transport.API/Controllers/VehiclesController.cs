using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Vehicles.Commands.CreateVehicle;
using Transport.Application.Features.Vehicles.Commands.DeleteVehicle;
using Transport.Application.Features.Vehicles.Commands.UpdateVehicle;
using Transport.Application.Features.Vehicles.Queries.GetAllVehicles;
using Transport.Application.Features.Vehicles.Queries.GetVehicleById;
using Transport.Application.Features.Vehicles.Queries.GetVehicleByPlateNumber;
using Transport.Application.Features.Vehicles.Queries.GetVehiclesByStatus;
using Transport.Domain.Enums;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VehiclesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleCommand command)
        {
            var id = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllVehiclesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetVehicleByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("plate/{plateNumber}")]
        public async Task<IActionResult> GetByPlateNumber(string plateNumber)
        {
            var result = await _mediator.Send(new GetVehicleByPlateNumberQuery(plateNumber));
            return Ok(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(VehicleStatus status)
        {
            var result = await _mediator.Send(new GetVehiclesByStatusQuery(status));
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteVehicleCommand { Id = id });
            return NoContent();
        }
    }
}
