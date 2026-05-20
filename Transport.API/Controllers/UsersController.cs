using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Users.Commands.ActivateUser;
using Transport.Application.Features.Users.Commands.DeactivateUser;
using Transport.Application.Features.Users.Commands.ResetUserPassword;
using Transport.Application.Features.Users.Commands.UpdateUser;
using Transport.Application.Features.Users.Queries.GetAllUsers;
using Transport.Application.Features.Users.Queries.GetUserById;
using Transport.Application.Features.Users.Queries.GetUsersByActive;
using Transport.Application.Features.Users.Queries.GetUsersByRole;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-role/{roleId:guid}")]
        public async Task<IActionResult> GetByRole(Guid roleId)
        {
            var result = await _mediator.Send(new GetUsersByRoleQuery(roleId));
            return Ok(result);
        }

        [HttpGet("by-active/{isActive:bool}")]
        public async Task<IActionResult> GetByActive(bool isActive)
        {
            var result = await _mediator.Send(new GetUsersByActiveQuery(isActive));
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id:guid}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivateUserCommand(id));
            return NoContent();
        }

        [HttpPut("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateUserCommand(id));
            return NoContent();
        }

        [HttpPut("{id:guid}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetUserPasswordCommand command)
        {
            command.UserId = id;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
