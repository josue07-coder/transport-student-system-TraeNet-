using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using Transport.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using Transport.Application.Features.Notifications.Queries.GetNotifications;
using Transport.Application.Features.Notifications.Queries.GetUnreadNotifications;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetNotificationsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            return Ok(await _mediator.Send(new GetUnreadNotificationsQuery()));
        }

        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _mediator.Send(new MarkNotificationAsReadCommand { Id = id });
            return NoContent();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _mediator.Send(new MarkAllNotificationsAsReadCommand());
            return NoContent();
        }
    }
}
