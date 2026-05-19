using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Features.Students.Commands.DeleteStudent;
using Transport.Application.Features.Students.Queries.GetStudentByCode;
using Transport.Application.Features.Students.Queries.GetStudentById;
using Transport.Application.Features.Students.Queries.GetStudents;
using Transport.Application.Features.Students.Queries.GetStudentsByGrade;
using Transport.Application.Features.Students.Queries.GetStudentsByGuardian;
using Transport.Application.Features.Students.Queries.GetStudentsBySchool;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllStudentsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetStudentByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _mediator.Send(new GetStudentByCodeQuery(code));
            return Ok(result);
        }

        [HttpGet("by-grade/{gradeId}")]
        public async Task<IActionResult> GetByGrade(Guid gradeId)
        {
            var result = await _mediator.Send(new GetStudentsByGradeQuery(gradeId));
            return Ok(result);
        }

        [HttpGet("by-school/{schoolId}")]
        public async Task<IActionResult> GetBySchool(Guid schoolId)
        {
            var result = await _mediator.Send(new GetStudentsBySchoolQuery(schoolId));
            return Ok(result);
        }

        [HttpGet("by-guardian/{guardianId}")]
        public async Task<IActionResult> GetByGuardian(Guid guardianId)
        {
            var result = await _mediator.Send(new GetStudentsByGuardianQuery(guardianId));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteStudentCommand { Id = id });
            return NoContent();
        }
    }
}
