using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.SystemSettings.Commands.CreateSystemSetting;
using Transport.Application.Features.SystemSettings.Commands.DeleteSystemSetting;
using Transport.Application.Features.SystemSettings.Commands.UpdateSystemSetting;
using Transport.Application.Features.SystemSettings.Queries.GetAllSystemSettings;
using Transport.Application.Features.SystemSettings.Queries.GetSystemSettingById;
using Transport.Application.Features.SystemSettings.Queries.GetSystemSettingByKey;
using Transport.Application.Features.SystemSettings.Queries.GetSystemSettingsByCategory;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/system-settings")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllSystemSettingsQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSystemSettingByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var result = await _mediator.Send(new GetSystemSettingByKeyQuery(key));
            return Ok(result);
        }

        [HttpGet("by-category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var result = await _mediator.Send(new GetSystemSettingsByCategoryQuery(category));
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSystemSettingCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSystemSettingCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteSystemSettingCommand(id));
            return NoContent();
        }
    }
}
