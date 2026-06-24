using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.NonSchoolDays.Commands.CreateNonSchoolDay;
using Transport.Application.Features.NonSchoolDays.Commands.DeactivateNonSchoolDay;
using Transport.Application.Features.NonSchoolDays.Commands.DeleteNonSchoolDay;
using Transport.Application.Features.NonSchoolDays.Commands.UpdateNonSchoolDay;
using Transport.Application.Features.NonSchoolDays.Queries.GetActiveNonSchoolDayForDate;
using Transport.Application.Features.NonSchoolDays.Queries.GetAllNonSchoolDays;
using Transport.Application.Features.NonSchoolDays.Queries.GetNonSchoolDayById;
using Transport.Application.Features.NonSchoolDays.Queries.GetNonSchoolDaysByDateRange;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/non-school-days")]
    public class NonSchoolDaysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NonSchoolDaysController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllNonSchoolDaysQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetNonSchoolDayByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] GetNonSchoolDaysByDateRangeQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive([FromQuery] GetActiveNonSchoolDayForDateQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNonSchoolDayCommand command)
        {
            var id = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new { Id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNonSchoolDayCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateNonSchoolDayCommand { Id = id });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteNonSchoolDayCommand { Id = id });
            return NoContent();
        }
    }
}
