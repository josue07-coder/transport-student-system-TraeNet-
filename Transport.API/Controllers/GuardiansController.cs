using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Guardians.Commands.CreateGuardian;
using Transport.Application.Features.Guardians.Commands.DeleteGuardian;
using Transport.Application.Features.Guardians.Commands.UpdateGuardian;
using Transport.Application.Features.Guardians.Queries.GetGuardianByDocument;
using Transport.Application.Features.Guardians.Queries.GetGuardianById;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuardiansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GuardiansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGuardianCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllGuardiansQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetGuardianByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("document/{documentNumber}")]
        public async Task<IActionResult> GetByDocument(string documentNumber)
        {
            var result = await _mediator.Send(new GetGuardianByDocumentQuery(documentNumber));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGuardianCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteGuardianCommand { Id = id });
            return NoContent();
        }
    }
}
