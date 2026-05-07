using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Schools.Commands.CreateSchool;
using Transport.Application.Features.Schools.Queries.GetAllSchools;

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
        public async Task<IActionResult> Create(CreateSchoolCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllSchoolsQuery());
            return Ok(result);
        }
    }
}