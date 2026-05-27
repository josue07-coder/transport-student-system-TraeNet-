using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Me.Commands.ChangePassword;
using Transport.Application.Features.Me.Commands.UpdateMeProfile;
using Transport.Application.Features.Me.Commands.UpdateProfilePhoto;
using Transport.Application.Features.Me.Queries.GetMe;
using Transport.Application.Features.Me.Queries.GetMyRouteAssignments;
using Transport.Application.Features.Me.Queries.GetMyStudents;
using Transport.Application.Features.Me.Queries.GetMyTrips;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/me")]
    public class MeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            var result = await _mediator.Send(new GetMeQuery());
            return Ok(result);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetMyStudents()
        {
            var result = await _mediator.Send(new GetMyStudentsQuery());
            return Ok(result);
        }

        [HttpGet("route-assignments")]
        public async Task<IActionResult> GetMyRouteAssignments()
        {
            var result = await _mediator.Send(new GetMyRouteAssignmentsQuery());
            return Ok(result);
        }

        [HttpGet("trips")]
        public async Task<IActionResult> GetMyTrips()
        {
            var result = await _mediator.Send(new GetMyTripsQuery());
            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateMeProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("photo")]
        public async Task<IActionResult> UpdatePhoto([FromBody] UpdateProfilePhotoCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
