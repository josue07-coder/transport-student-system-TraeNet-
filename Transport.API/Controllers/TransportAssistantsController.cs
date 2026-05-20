using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.TransportAssistants.Commands.CreateTransportAssistant;
using Transport.Application.Features.TransportAssistants.Commands.DeleteTransportAssistant;
using Transport.Application.Features.TransportAssistants.Commands.UpdateTransportAssistant;
using Transport.Application.Features.TransportAssistants.Queries.GetAllTransportAssistants;
using Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantByDocument;
using Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantById;
using Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantsByActive;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/transport-assistants")]
    public class TransportAssistantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransportAssistantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransportAssistantCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTransportAssistantsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetTransportAssistantByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("document/{documentNumber}")]
        public async Task<IActionResult> GetByDocument(string documentNumber)
        {
            var result = await _mediator.Send(new GetTransportAssistantByDocumentQuery(documentNumber));
            return Ok(result);
        }

        [HttpGet("by-active/{isActive}")]
        public async Task<IActionResult> GetByActive(bool isActive)
        {
            var result = await _mediator.Send(new GetTransportAssistantsByActiveQuery(isActive));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransportAssistantCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteTransportAssistantCommand { Id = id });
            return NoContent();
        }
    }
}
