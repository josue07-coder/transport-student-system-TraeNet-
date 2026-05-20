using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Roles.Commands.AssignPermissionToRole;
using Transport.Application.Features.Roles.Commands.RemovePermissionFromRole;
using Transport.Application.Features.Roles.Commands.UpdateRole;
using Transport.Application.Features.Roles.Queries.GetAllRoles;
using Transport.Application.Features.Roles.Queries.GetRoleById;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId)
        {
            await _mediator.Send(new AssignPermissionToRoleCommand(roleId, permissionId));
            return NoContent();
        }

        [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId)
        {
            await _mediator.Send(new RemovePermissionFromRoleCommand(roleId, permissionId));
            return NoContent();
        }
    }
}
