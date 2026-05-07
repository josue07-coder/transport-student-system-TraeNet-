using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Features.Students.Commands.DeleteStudent;
using Transport.Application.Features.Students.Queries.GetStudents;

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

        //  POST: api/students
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        //  GET: api/students
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllStudentsQuery());
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateStudentCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);

            return NoContent();
        }

        //  DELETE: api/students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteStudentCommand { Id = id });
            return NoContent();
        }
    }
}