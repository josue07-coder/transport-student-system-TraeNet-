using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Backups.Commands.CreateManualBackup;
using Transport.Application.Features.Backups.Commands.RestoreBackup;
using Transport.Application.Features.Backups.Queries.GetAllBackups;
using Transport.Application.Features.Backups.Queries.GetBackupById;
using Transport.Application.Features.Backups.Queries.GetLatestBackup;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/backups")]
    public class BackupsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("manual")]
        public async Task<IActionResult> CreateManual()
        {
            var result = await _mediator.Send(new CreateManualBackupCommand());
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllBackupsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var result = await _mediator.Send(new GetLatestBackupQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetBackupByIdQuery(id));
            return Ok(result);
        }

        [HttpPost("{id:guid}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            await _mediator.Send(new RestoreBackupCommand(id));
            return NoContent();
        }
    }
}
