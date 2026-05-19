using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Schools.Commands.CreateSchool;
using Transport.Application.Features.Schools.Commands.DeleteSchool;
using Transport.Application.Features.Schools.Commands.UpdateSchool;
using Transport.Application.Features.Schools.Queries.GetAllSchools;
using Transport.Application.Features.Schools.Queries.GetSchoolById;
using Transport.Application.Features.Schools.Queries.GetSchoolsBySector;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchoolsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSchoolCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllSchoolsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSchoolByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("by-sector/{sectorId}")]
        public async Task<IActionResult> GetBySector(Guid sectorId)
        {
            var result = await _mediator.Send(new GetSchoolsBySectorQuery(sectorId));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSchoolCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteSchoolCommand { Id = id });
            return NoContent();
        }
    }
}
