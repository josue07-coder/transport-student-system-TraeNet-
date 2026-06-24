using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Grades.Commands.CreateGrade;
using Transport.Application.Features.Grades.Commands.DeleteGrade;
using Transport.Application.Features.Grades.Commands.UpdateGrade;
using Transport.Application.Features.Grades.Queries.GetGradeById;
using Transport.Application.Features.Grades.Queries.GetGradesBySchool;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/[controller]")]
    public class GradesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GradesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGradeCommand command)
        {
            var id = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllGradesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGradeByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-school/{schoolId}")]
        public async Task<IActionResult> GetBySchool(Guid schoolId)
        {
            var result = await _mediator.Send(new GetGradesBySchoolQuery(schoolId));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGradeCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteGradeCommand { Id = id });
            return NoContent();
        }
    }
}
