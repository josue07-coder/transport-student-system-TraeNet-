using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.AuditLogs.Queries.GetAllAuditLogs;
using Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByAction;
using Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByDateRange;
using Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByEntity;
using Transport.Application.Features.AuditLogs.Queries.GetAuditLogsByUser;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/audit-logs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditLogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAuditLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var result = await _mediator.Send(new GetAuditLogsByUserQuery(userId));
            return Ok(result);
        }

        [HttpGet("by-entity/{entityName}/{entityId}")]
        public async Task<IActionResult> GetByEntity(string entityName, string entityId)
        {
            var result = await _mediator.Send(new GetAuditLogsByEntityQuery(entityName, entityId));
            return Ok(result);
        }

        [HttpGet("by-action/{action}")]
        public async Task<IActionResult> GetByAction(string action)
        {
            var result = await _mediator.Send(new GetAuditLogsByActionQuery(action));
            return Ok(result);
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetAuditLogsByDateRangeQuery(startDate, endDate));
            return Ok(result);
        }
    }
}
